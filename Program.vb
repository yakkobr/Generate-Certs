Imports System.IO
Imports System.Net
Imports System.Reflection
Imports System.Runtime.InteropServices

Module Program

    Dim openssl_manual_path As String
    Dim destdir As String
    Dim slash As String

    Const OutputDirectoryName As String = "SSL_Certs_Out"

    Const CACertName As String = "ca"
    Const CertificateCertName As String = "certificate"
    Const ServerCertName As String = "server"
    Const ClientCertName As String = "client"
    Const CertValidityDays As String = "3650"

    Sub Main()
        'Set title
        Console.Title = Assembly.GetExecutingAssembly.GetName.Name

        'Set slash depending on OS
        If GetOS() = OSPlatform.Windows Then
            slash = "\"
        Else
            slash = "/"
        End If

        'Set destination directory
        destdir = Environment.CurrentDirectory & slash & OutputDirectoryName

retry_openssl_test:
        Try
            'Confirm openssl opens
            Dim o As New Process
            If openssl_manual_path Is Nothing Then
                o.StartInfo.FileName = "openssl"
            Else
                o.StartInfo.FileName = openssl_manual_path
            End If
            o.StartInfo.Arguments = "version"
            o.StartInfo.UseShellExecute = False
            o.StartInfo.CreateNoWindow = True
            o.StartInfo.RedirectStandardOutput = True
            o.Start()
        Catch ex As Exception
            If GetOS() <> OSPlatform.Windows Then
                'Non-Windows
                Console.WriteLine("Could not detect OpenSSL. Please install it first and then run Generate-Certs.")
                Console.WriteLine()
                Console.WriteLine("Try this command:")
                Console.WriteLine("    apt update && apt install openssl -y")
                Console.WriteLine("or try:")
                Console.WriteLine("    yum install openssl")
                Console.WriteLine()
                Console.WriteLine("If that does not work, try the instructions here:")
                Console.WriteLine("  http://bit.ly/linux-openssl")
                Console.ReadLine()
                End
            End If

            'Windows
            Console.WriteLine("Could not detect OpenSSL. What would you like to do?")
            Console.WriteLine("(1) Download and install OpenSSL for me")
            Console.WriteLine("(2) Specify a path to OpenSSL.exe")
            Console.WriteLine()
            Dim todo As String = Console.ReadLine()
            Console.WriteLine()

            If todo = "1" Then
                'Set temp file path
                Dim tf = Path.GetTempFileName & ".exe"

                Try
                    'Download OpenSSL installer
                    Dim url As String = "https://slproweb.com/download/Win32OpenSSL-1_1_1i.exe"
                    Dim wc As New WebClient
                    Console.WriteLine("Downloading, this should take less than 5 minutes...")
                    wc.DownloadFile(url, tf)
                    Console.WriteLine("Download complete, installing, this should take less than 5 minutes...")
                Catch ex_download As Exception
                    Try
                        File.Delete(tf)
                    Catch ex_download_delete As Exception
                    End Try
                    Console.WriteLine("Error downloading OpenSSL. Possible firewall or content filtering issue?")
                    Console.ReadLine()
                    End
                End Try

                Try
                    'Install OpenSSL
                    Dim pp As New Process
                    pp.StartInfo.FileName = tf
                    pp.StartInfo.Arguments = "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-"
                    pp.Start()
                    Do Until pp.HasExited = True

                    Loop

                    Dim OpenSSL_Dir As String = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) & "\OpenSSL-Win32\bin"

                    'Set PATH var on the system so that you can type OpenSSL in command prompt in any directory
                    Dim p = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine)
                    Environment.SetEnvironmentVariable("PATH", p & ";" & OpenSSL_Dir, EnvironmentVariableTarget.Machine)

                    'Set path to OpenSSL
                    openssl_manual_path = OpenSSL_Dir & "\OpenSSL.exe"

                    'Delete downloaded file
                    Try
                        File.Delete(tf)
                    Catch ex2 As Exception
                    End Try

                    'Report installed
                    Console.WriteLine("Installation complete, checking for OpenSSL...")
                    Console.WriteLine()

                    'Retry OpenSSL test
                    GoTo retry_openssl_test
                Catch ex_install As Exception
                    Console.WriteLine("Error installing OpenSSL. Are you running as Administrator?")
                    Console.ReadLine()
                    End
                End Try
                GoTo retry_openssl_test
            ElseIf todo = "2" Then
retry_specifypath:
                Console.WriteLine()
                Console.WriteLine("Please specify path to OpenSSL.exe:")
                Dim osp = Console.ReadLine()
                Console.WriteLine()
                If osp Is Nothing Then
                    GoTo retry_specifypath
                Else
                    If File.Exists(osp) Then
                        openssl_manual_path = osp
                        GoTo retry_openssl_test
                    Else
                        Console.WriteLine("The path you specified does not exist, please try again.")
                        GoTo retry_specifypath
                    End If
                End If
            Else
                GoTo retry_openssl_test

            End If
        End Try

        Console.Clear()

        'Information
        Console.WriteLine("################### WARNING ######################")
        Console.WriteLine("Generate-Certs generates self-signed SSL certificates")
        Console.WriteLine()
        Console.WriteLine("The following folder will be used to store the certificates:")
        Console.WriteLine("    " & destdir)
        Console.WriteLine("Any existing data in the directory shown above will be erased and overwritten after pressing ENTER")
        Console.WriteLine()
        Console.WriteLine("Press Ctrl-C to terminate the app if you do not want the above files to be deleted")
        Console.WriteLine("##################################################")
        Console.WriteLine()
        Console.WriteLine("Press enter to continue")
        Console.ReadLine()
        Console.WriteLine()

        'Delete existing certificates
        ClearOutputFolder()

        'Create folder if needed
        If Directory.Exists(destdir) = False Then
            Directory.CreateDirectory(destdir)
        End If

        Console.Clear()

oneortwo_retry:
        Console.WriteLine()
        Console.WriteLine("Do you want one certificate (certificate.crt) or two certificates (server.crt and client.crt)?")
        Console.WriteLine("A root certificate authority is created either way.")
        Console.WriteLine("(1) one certificates (2) two certificates")
        Dim oneortwo_var As String = Nothing
        Dim oneortwo_line As String = Console.ReadLine
        If oneortwo_line = "1" Then
            oneortwo_var = "1"
        ElseIf oneortwo_line = "2" Then
            oneortwo_var = "2"
        Else
            GoTo oneortwo_retry
        End If

        Console.WriteLine()
        Console.WriteLine("Please create passwords for your certs or press enter to not set a password")
        Console.WriteLine("Root CA Cert password is REQUIRED if you want PFX and PEM files created (for all files), otherwise optional")
        Console.WriteLine()

        Dim rootca_pw As String = Nothing
        Dim server_pw As String = Nothing
        Dim client_pw As String = Nothing
        Dim certificate_pw As String = Nothing

        Console.Write("Root CA Cert Password: ")
        rootca_pw = Console.ReadLine
        If oneortwo_var = "1" Then
            Console.Write("Certificate Password: ")
            certificate_pw = Console.ReadLine
        Else
            Console.Write("Server Cert Password:  ")
            server_pw = Console.ReadLine
            Console.Write("Client Cert Password:  ")
            client_pw = Console.ReadLine
        End If

subject_retry:
        Console.WriteLine()
        Console.WriteLine("What is the SUBJECT of the certificate?")
        Console.WriteLine("Usually this is the hostname that you'll connect over, such as localhost or your.domain.com")
        Console.WriteLine("If you want to enter an IP address, wait until the next question")
        Console.WriteLine("Press enter to set to localhost")
        Console.WriteLine()
        Dim cn As String = Nothing
        Dim subj_var As String = Nothing
        Dim subj_line As String = Console.ReadLine
        If subj_line.Length > 0 Then
            subj_var = subj_line
        Else
            subj_var = "localhost"
        End If
        cn = "CN = " & subj_var

san_retry:
        Console.WriteLine()
        Console.WriteLine("What is the SUBJECT ALTERNATIVE NAME (SAN) of the certificate?")
        Console.WriteLine("Usually this is an alternate hostname such as 127.0.0.1 or alt.domain.com")
        Console.WriteLine("If you are using the certificates to connect over an IP address rather than a hostname, enter that IP address here")
        Console.WriteLine("Press enter to skip")
        Console.WriteLine()
        Dim san As String = Nothing
        Dim san_var As String = Nothing
        Dim san_line As String = Console.ReadLine
        If san_line.Length > 0 Then
            Try
                Dim parsedIP As String = IPAddress.Parse(san_line).ToString
                If parsedIP IsNot Nothing Then
                    san_var = "IP.1 = " & parsedIP
                End If
            Catch ex As Exception

            End Try
            If san_var = Nothing Then
                san_var = "DNS.2 = " & san_line
            End If
        End If
        san = "DNS.1 = " & subj_var & vbCrLf & san_var

        'Write temp files
        File.WriteAllText(destdir & "/generate-certs-serial", "00" & vbCrLf)
        File.WriteAllText(destdir & "/generate-certs-db", "")
        File.WriteAllText(destdir & "/generate-certs-ca.conf", GetOpenSslCfg().Replace("{SAN}", "").Replace("{CN}", "CN = Generate-Certs Root CA").Replace("{ALT}", "").Replace("{CACERTNAME}", CACertName).Replace("{BC}", "critical,CA:true").Replace("{KU}", "nonRepudiation, digitalSignature, keyEncipherment, cRLSign, keyCertSign").Replace("{CERTVALIDITYDAYS}", CertValidityDays))
        File.WriteAllText(destdir & "/generate-certs-certs.conf", GetOpenSslCfg().Replace("{SAN}", "subjectAltName = @alt_names").Replace("{CN}", cn).Replace("{ALT}", san).Replace("{CACERTNAME}", CACertName).Replace("{BC}", "CA:false").Replace("{KU}", "nonRepudiation, digitalSignature, keyEncipherment").Replace("{CERTVALIDITYDAYS}", CertValidityDays))

        'Begin writing cert data

        'Root CA Certificate
        GenerateSection("Root CA Certificate")
        GenerateRootCACertificate(CACertName, rootca_pw)

        If oneortwo_var = "1" Then
            'One certificate
            GenerateSection("Certificate")
            GenerateCertificate(CertificateCertName, certificate_pw, rootca_pw)

        ElseIf oneortwo_var = "2" Then
            'Two certificates

            'Server certificate
            GenerateSection("Server Certificate")
            GenerateCertificate(ServerCertName, server_pw, rootca_pw)

            'Client certificate
            GenerateSection("Client Certificate")
            GenerateCertificate(ClientCertName, client_pw, rootca_pw)
        End If

        GenerateSection("RESULTS")
        If rootca_pw.Length = 0 Then
            Console.WriteLine("WARNING: Root CA password not specified, PFX and PEM files NOT created")
            Console.WriteLine()
        End If
        Console.WriteLine("Certificates have been generated!")
        Console.WriteLine()
        Console.WriteLine("Don't forget the passwords you used for the certificates:")
        Console.WriteLine("Root CA Cert Password = " & rootca_pw)
        If oneortwo_var = "1" Then
            Console.WriteLine("Certificate Password  = " & certificate_pw)
        ElseIf oneortwo_var = "2" Then
            Console.WriteLine("Server Cert Password  = " & server_pw)
            Console.WriteLine("Client Cert Password  = " & client_pw)
        End If
        Console.WriteLine()
        Console.WriteLine("Certificates stored in:")
        Console.WriteLine("    " & destdir)
        Console.WriteLine("##################################################")
        Console.WriteLine()
        Console.WriteLine()

        DeleteTempCertFiles()

        Console.ReadLine()
        End
    End Sub

    ''' <summary>
    ''' Runs the openssl process with arguments
    ''' </summary>
    ''' <param name="args"></param>
    Private Sub openssl(args As String)
        Try
            Dim o As New Process
            o.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            If openssl_manual_path Is Nothing Then
                o.StartInfo.FileName = "openssl"
            Else
                o.StartInfo.FileName = openssl_manual_path
            End If
            o.StartInfo.Arguments = args
            If Directory.Exists(destdir) = False Then Directory.CreateDirectory(destdir)
            o.StartInfo.WorkingDirectory = destdir
            'o.StartInfo.RedirectStandardOutput = True
            'o.StartInfo.RedirectStandardError = True
            o.Start()

            Do While o.HasExited = False
                Threading.Thread.Sleep(50)
            Loop
        Catch ex As Exception
            Console.WriteLine("Error launching openssl, are you sure it's set up correctly and accessible from the command line?")
        End Try
    End Sub

    Sub ClearOutputFolder()
        Try
            Dim f As New DirectoryInfo(destdir)
            Dim fiArr As FileInfo() = f.GetFiles()
            Dim fri As FileInfo
            For Each fri In fiArr
                Try
                    fri.Delete()
                Catch ex As Exception
                End Try
            Next
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Deletes temporary certificate files
    ''' </summary>
    Private Sub DeleteTempCertFiles()
        Dim certfiles As String() = {
            "00.pem", "01.pem", "02.pem", "03.pem", "04.pem", "05.pem",
            "generate-certs-db", "generate-certs-db.attr", "generate-certs-db.attr.old", "generate-certs-db.old",
            "generate-certs-serial", "generate-certs-serial.old",
            "generate-certs-ca.conf", "generate-certs-certs.conf"
            }

        DeleteIfExists(certfiles)
    End Sub

    Sub DeleteIfExists(path As String())
        For Each p As String In path
            If File.Exists(destdir & slash & p) Then
                Try
                    File.Delete(destdir & slash & p)
                Catch ex As Exception
                End Try
            End If
        Next
    End Sub

    Sub GenerateSection(title As String)
        Console.WriteLine()
        Console.WriteLine("##################################################")
        Console.WriteLine(title)
        Console.WriteLine("##################################################")
        Console.WriteLine()
    End Sub

    Function GetOS() As OSPlatform
        Dim OS As OSPlatform

        If RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
            OS = OSPlatform.Windows
        ElseIf RuntimeInformation.IsOSPlatform(OSPlatform.Linux) Then
            OS = OSPlatform.Linux
        ElseIf RuntimeInformation.IsOSPlatform(OSPlatform.OSX) Then
            OS = OSPlatform.OSX
        End If

        Return OS
    End Function

    ''' <summary>
    ''' Generates individual root CA certificate
    ''' </summary>
    ''' <param name="certname"></param>
    ''' <param name="pw"></param>
    Sub GenerateRootCACertificate(certname As String, pw As String)
        openssl("ecparam -genkey -name prime256v1 -out ""{CERTNAME}.key""".Replace("{CERTNAME}", certname))
        openssl("req -x509 -config generate-certs-ca.conf -new -SHA256 -nodes -key ""{CERTNAME}.key"" -out ""{CERTNAME}.crt"" -days {CERTVALIDITYDAYS}".Replace("{CERTNAME}", certname).Replace("{CERTVALIDITYDAYS}", CertValidityDays))
        openssl("ca -config generate-certs-ca.conf -batch -selfsign -in ""{CERTNAME}.csr"" -out ""{CERTNAME}.crt"" -days {CERTVALIDITYDAYS}".Replace("{CERTNAME}", certname).Replace("{CERTVALIDITYDAYS}", CertValidityDays))

        If pw.Length > 0 Then
            openssl("pkcs12 -export -passout pass:""{0}"" -inkey ""{CERTNAME}.key"" -in ""{CERTNAME}.crt"" -out ""{CERTNAME}.pfx""".Replace("{0}", pw).Replace("{CERTNAME}", certname))
        End If

        Console.WriteLine()
    End Sub

    ''' <summary>
    ''' Generates individual certificate
    ''' </summary>
    ''' <param name="certname"></param>
    ''' <param name="pw"></param>
    ''' <param name="rootcapw"></param>
    Sub GenerateCertificate(certname As String, pw As String, rootcapw As String)
        openssl("ecparam -genkey -name prime256v1 -out ""{CERTNAME}.key""".Replace("{CERTNAME}", certname))
        openssl("req -config generate-certs-certs.conf -new -SHA256 -key ""{CERTNAME}.key"" -nodes -out ""{CERTNAME}.csr"" -days {CERTVALIDITYDAYS}".Replace("{CERTNAME}", certname).Replace("{CERTVALIDITYDAYS}", CertValidityDays))
        openssl("ca -config generate-certs-certs.conf -batch -in ""{CERTNAME}.csr"" -out ""{CERTNAME}.crt"" -days {CERTVALIDITYDAYS}".Replace("{CERTNAME}", certname).Replace("{CERTVALIDITYDAYS}", CertValidityDays))

        If rootcapw.Length > 0 Then
            openssl("pkcs12 -export -passout pass:""{0}"" -inkey ""{CERTNAME}.key"" -in ""{CERTNAME}.crt"" -out ""{CERTNAME}.pfx""".Replace("{0}", pw).Replace("{CERTNAME}", certname))
        End If

        Console.WriteLine()
    End Sub

    Function GetOpenSslCfg() As String
        Return "[ca]
default_ca = CA_default

[CA_default]
dir = .
database = $dir/generate-certs-db
new_certs_dir = $dir/
serial = $dir/generate-certs-serial
private_key = ./{CACERTNAME}.key
certificate = ./{CACERTNAME}.crt
default_days = {CERTVALIDITYDAYS}
default_md = sha256
policy = policy_match
copy_extensions = copyall
unique_subject	= no

[policy_match]
countryName = optional
stateOrProvinceName = optional
localityName = optional
organizationName = optional
organizationalUnitName = optional
commonName = supplied
emailAddress = optional

[req]
prompt = no
distinguished_name = req_distinguished_name
req_extensions = v3_data
x509_extensions	= v3_data

[req_distinguished_name]
OU = Created by Generate-Certs
O = Created by Generate-Certs
{CN}

[v3_data]
{SAN}
basicConstraints = {BC}
keyUsage = {KU}
subjectKeyIdentifier = hash

[alt_names]
{ALT}
"
    End Function
End Module