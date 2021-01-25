
# Generate Self-Signed SSL Certificates

This is a Windows/Linux app (x64) that generates a set of self-signed SSL certificates, including the Root CA cert.

### Why use Generate-Certs?
1 minute to use this program compared to 30 minutes entering in and adjusting all the commands and files

### Notes
 
- The Root CA cert requires a password **if you want PFX and PEM files created** (for all files).
	-  All certificates - password is **optional**.
- Most people use **2048 as the key size** as it is still considered secure.
- The **SUBJECT** name is usually the main hostname that you're connecting to.
- The **SUBJECT ALTERNATIVE NAME** is usually an alternate name, or an IP address.
	- **Generate-Certs** supports the detection of IP addresses.
- Certificates expire **3650 days** after day of certificate generation.
- Files are created in the **SSL_Certs_Out** subdirectory from whatever directory you're in when running **Generate-Certs**.  It will tell you the path once you start the program.

## Install

### Windows

[Download and install .NET 5 for Windows x64](https://dotnet.microsoft.com/download/dotnet/current/runtime)

Download **Generate-Certs.exe** from the [Releases](<https://github.com/asheroto/Generate-Certs/releases>) page, then open Generate-Certs.exe or run it from the command line.

### Linux

Install .NET 5 for Linux x64:
```
wget dot.net/v1/dotnet-install.sh -O dotnet.sh
bash dotnet.sh -c 5.0
```

Run Generate-Certs:
```
wget https://github.com/asheroto/Generate-Certs/releases/download/0.0.19/Generate-Certs
chmod +x Generate-Certs
./Generate-Certs
```

# Screenshots

## Info/Warning
![Generate-Certs Initial Screen](https://github.com/asheroto/Generate-Certs/blob/master/screenshots/1.png)

## Configuration (Cert Passwords + Key Size)
![Generate-Certs Configuration](https://github.com/asheroto/Generate-Certs/blob/master/screenshots/2.png)

## Final Result
![Final Result](https://github.com/asheroto/Generate-Certs/blob/master/screenshots/3.png)

## Created Files
![Generate-Certs Result](https://github.com/asheroto/Generate-Certs/blob/master/screenshots/4.png)