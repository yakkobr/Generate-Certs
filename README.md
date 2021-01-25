# Generate Self-Signed SSL Certificates

This is a Windows/Linux app (x64) that generates a set of self-signed SSL certificates

Takes 1 minute to use this program compared to 15-30 minutes entering in and adjusting all the commands
 
- You will be asked to set the password for the Root CA cert, the Server Cert, and the Client cert. The Root CA cert requires a password if you want PFX and PEM files created, server/client password is optional.
- You will also be asked what key size to use for the certs. Most people use 2048 as it is still considered secure.
- Certificates expire 3650 days after day of certificate generation.
- Files are created in the SSL_Certs subdirectory from where Generate-Certs program is lauched.

## Windows

[Download and install .NET 5 for Windows x64](https://dotnet.microsoft.com/download/dotnet/current/runtime)

Download Generate-Certs.exe from the [Releases](<https://github.com/asheroto/Generate-Certs/releases>) page, then open Generate-Certs.exe or run it from the command line.

## Linux

Install .NET 5 for Linux x64:
```
wget dot.net/v1/dotnet-install.sh -O dotnet.sh
bash dotnet.sh -c 5.0
```

Run Generate-Certs:
```
wget https://github.com/asheroto/Generate-Certs/releases/download/0.0.18/Generate-Certs
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