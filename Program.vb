Imports System.IO
Imports System.Net
Imports System.Runtime.InteropServices

Module Program

    Dim openssl_manual_path As String
    Dim destdir As String

    Sub Main()
        If GetOS() = "Windows" Then
            destdir = Environment.CurrentDirectory & "\SSL_Certs"
        Else
            destdir = Environment.CurrentDirectory & "/SSL_Certs"
        End If

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
            If GetOS() <> "Windows" Then
                'Non-Windows
                Console.WriteLine("Could not detect OpenSSL. Please install it first and then Generate-Certs.")
                Console.WriteLine()
                Console.WriteLine("Try this command:")
                Console.WriteLine("  apt update && apt install openssl -y")
                Console.WriteLine()
                Console.WriteLine("If that does not work, try the instructions here:")
                Console.WriteLine("  http://bit.ly/install-openssl")
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
                Try
                    'Set temp file path
                    Dim tf = Path.GetTempFileName & ".exe"

                    'Download installer
                    Dim url As String = "https://slproweb.com/download/Win32OpenSSL-1_1_1i.exe"
                    Dim wc As New WebClient
                    Console.WriteLine("Downloading, this should take less than 5 minutes...")
                    wc.DownloadFile(url, tf)
                    Console.WriteLine("Download complete, installing, this should take less than 5 minutes...")
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
                Catch ex2 As Exception
                    Console.WriteLine("Error downloading OpenSSL. Please download and install it manually then run Generate-Certs again.")
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
        Console.WriteLine("Generate-Certs generates client & server SSL certificates")
        Console.WriteLine()
        Console.WriteLine("The following folder will be used to store the certificates:")
        Console.WriteLine(destdir)
        Console.WriteLine("Any existing certificate data in the directory shown above will be erased and overwritten")
        Console.WriteLine()
        Console.WriteLine("List of files to be deleted if they exist:")
        Console.WriteLine("     client.crt client.pem client.pfx client.csr client.key client-secret.key")
        Console.WriteLine("     server.crt server.pem server.pfx server.key server-secret.key server.csr")
        Console.WriteLine("     ca.key ca.pem ca.pfx ca-secret.key ca.crt ")
        Console.WriteLine()
        Console.WriteLine("Press Ctrl-C to terminate the app if you do not want the above files to be deleted")
        Console.WriteLine("##################################################")
        Console.WriteLine()
        Console.WriteLine("Press enter to continue")
        Console.ReadLine()
        Console.WriteLine()

        'Delete existing certificates
        DeleteCertFiles()

        Console.Clear()

        'Configuration - cert passwords & key size
        Console.WriteLine("############### CONFIGURATION ###################")
        Console.WriteLine("Please create passwords for your certs, or just press enter to not set a password")
        Console.WriteLine()
        Console.WriteLine("Root CA Cert password is REQUIRED if you want PFX and PEM files created (for all files), otherwise optional")
        Console.WriteLine("##################################################")

        Dim rootca_pw As String
        Dim server_pw As String
        Dim client_pw As String

        Console.Write("Root CA Cert Password: ")
        rootca_pw = Console.ReadLine
        Console.Write("Server Cert Password:  ")
        server_pw = Console.ReadLine
        Console.Write("Client Cert Password:  ")
        client_pw = Console.ReadLine

keysize_retry:
        Console.WriteLine()
        Console.WriteLine("What key size do you want? I recommend either 2048 Or 4096.")
        Console.WriteLine("The higher the number the more secure. 2048 is still considered very secure and is the most common choice.")
        Console.WriteLine("(1) 1024 (2) 2048 (3) 3072 (4) 4096")
        Dim keysize_var As String = Nothing
        Dim keysize_line As String = Console.ReadLine()
        If keysize_line = "1" Then
            keysize_var = "1024"
        ElseIf keysize_line = "2" Then
            keysize_var = "2048"
        ElseIf keysize_line = "3" Then
            keysize_var = "3072"
        ElseIf keysize_line = "4" Then
            keysize_var = "4096"
        Else
            Console.WriteLine()
            Console.WriteLine("Sorry, I didn't understand your input, please try again")
            GoTo keysize_retry
        End If

        Console.WriteLine()

        Console.WriteLine("##################################################")
        Console.WriteLine("Root CA Certificate")
        Console.WriteLine("##################################################")

        openssl("genrsa -passout pass:""{0}"" -out ca-secret.key {1}".Replace("{0}", rootca_pw).Replace("{1}", keysize_var))
        openssl("rsa -passin pass:""{0}"" -in ca-secret.key -out ca.key".Replace("{0}", rootca_pw))
        openssl("req -x509 -new -days 3650 -key ca.key -sha256 -out ca.crt -subj ""/CN=localhost"" -addext ""subjectAltName=DNS:localhost,IP:127.0.0.1""")
        If rootca_pw.Length > 0 Then
            openssl("pkcs12 -export -passout pass:""{0}"" -inkey ca.key -in ca.crt -out ca.pfx".Replace("{0}", rootca_pw))
            openssl("pkcs12 -passin pass:""{0}"" -passout pass:""{0}"" -in ca.pfx -out ca.pem".Replace("{0}", rootca_pw))
        End If
        Console.WriteLine()

        Console.WriteLine("##################################################")
        Console.WriteLine("Server Certificate")
        Console.WriteLine("##################################################")

        openssl("genrsa -passout pass:""{0}"" -out server-secret.key {1}".Replace("{0}", server_pw).Replace("{1}", keysize_var))
        openssl("rsa -passin pass:""{0}"" -in server-secret.key -out server.key".Replace("{0}", server_pw))
        openssl("req -new -key server.key -out server.csr -subj ""/CN=localhost"" -addext ""subjectAltName=DNS:localhost,IP:127.0.0.1""")
        openssl("x509 -req -days 3650 -in server.csr -CA ca.crt -CAkey ca.key -set_serial 01 -out server.crt".Replace("{0}", server_pw))
        If rootca_pw.Length > 0 Then
            openssl("pkcs12 -export -passout pass:""{0}"" -inkey server.key -in server.crt -out server.pfx".Replace("{0}", server_pw))
            openssl("pkcs12 -passin pass:""{0}"" -passout pass:""{0}"" -in server.pfx -out server.pem".Replace("{0}", server_pw))
        End If
        Console.WriteLine()

        Console.WriteLine("##################################################")
        Console.WriteLine("Client Certificate")
        Console.WriteLine("##################################################")

        openssl("genrsa -passout pass:""{0}"" -out client-secret.key {1}".Replace("{0}", client_pw).Replace("{1}", keysize_var))
        openssl("rsa -passin pass:""{0}"" -in client-secret.key -out client.key".Replace("{0}", client_pw))
        openssl("req -new -key client.key -out client.csr -subj ""/CN=localhost"" -addext ""subjectAltName=DNS:localhost,IP:127.0.0.1""")
        openssl("x509 -req -days 3650 -in client.csr -CA ca.crt -CAkey ca.key -set_serial 01 -out client.crt".Replace("{0}", client_pw))
        If rootca_pw.Length > 0 Then
            openssl("pkcs12 -export -passout pass:""{0}"" -inkey client.key -in client.crt -out client.pfx".Replace("{0}", client_pw))
            openssl("pkcs12 -passin pass:""{0}"" -passout pass:""{0}"" -in client.pfx -out client.pem".Replace("{0}", client_pw))
        End If
        Console.WriteLine()

        Console.WriteLine("##################################################")
        If rootca_pw.Length = 0 Then
            Console.WriteLine("WARNING: Root CA password not specified, PFX and PEM files NOT created")
        End If
        Console.WriteLine("Done!")
        Console.WriteLine()
        Console.WriteLine("Don't forget the passwords you used for the certificates:")
        Console.WriteLine("Root CA Cert Password = " & rootca_pw)
        Console.WriteLine("Server Cert Password  = " & server_pw)
        Console.WriteLine("Client Cert Password  = " & client_pw)
        Console.WriteLine()
        Console.WriteLine("Certificates stored in:")
        Console.WriteLine(destdir)
        Console.WriteLine("##################################################")
        Console.WriteLine()

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
            o.Start()

            Do While o.HasExited = False
                Threading.Thread.Sleep(50)
            Loop
        Catch ex As Exception
            Console.WriteLine("Error launching openssl, are you sure it's set up correctly and accessible from the command line?")
        End Try
    End Sub
    ''' <summary>
    ''' Deletes all generated certificate files
    ''' </summary>
    Private Sub DeleteCertFiles()
        Dim certfiles As String() = {
            "client.crt", "client.pem", "client.pfx", "client.csr", "client.key", "client-secret.key",
            "server.crt", "server.pem", "server.pfx", "server.key", "server-secret.key", "server.csr",
            "ca.key", "ca.pem", "ca.pfx", "ca-secret.key", "ca.crt"
        }

        For Each cf As String In certfiles
            If File.Exists(destdir & "\" & cf) Then
                Try
                    File.Delete(destdir & "\" & cf)
                Catch ex As Exception

                End Try
            End If
        Next
    End Sub

    Function GetOS() As String
        Dim OS As String = Nothing

        If RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
            OS = "Windows"
        ElseIf RuntimeInformation.IsOSPlatform(OSPlatform.Linux) Then
            OS = "Linux"
        ElseIf RuntimeInformation.IsOSPlatform(OSPlatform.OSX) Then
            OS = "OSX"
        End If

        Return OS
    End Function
End Module