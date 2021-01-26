
# Generate Self-Signed SSL Certificates

This is a Windows/Linux app (x64) that generates a self-signed SSL certificates, including the Root CA cert.

### Files Created

**Option #1 - 1 certificate:**

![Generate-Certs Result - 1 cert](https://github.com/asheroto/Generate-Certs/blob/master/screenshots/4.png)

**Option #2 - 2 certificates:**

![Generate-Certs Result - 2 certs](https://github.com/asheroto/Generate-Certs/blob/master/screenshots/5.png)

### Why Use Generate-Certs?
1 minute to use this program compared to 30 minutes entering in and adjusting all the commands and files

### Notes

- [ECSDA certificates](https://blog.cloudflare.com/ecdsa-the-digital-signature-algorithm-of-a-better-internet/) by design (more secure than RSA) using the **secp256p1** curve
- **If you want PFX and PEM files created** (for all files), you MUST set a Root CA Cert password.
- The **SUBJECT** name is usually the main hostname that you're connecting to.
- The **SUBJECT ALTERNATIVE NAME** is usually an alternate name, or an IP address.
	- **Generate-Certs** supports the detection of IP addresses in the SAN field.
- Certificates expire **3650 days** after day of certificate generation.
- Files are created in the **SSL_Certs_Out** subdirectory from whatever directory you're in when running **Generate-Certs**.  It will tell you the path once you start the program.

## Install

### Windows

[Download and install .NET 5 for Windows x64](https://dotnet.microsoft.com/download/dotnet/current/runtime)

[Download Generate-Certs.exe](<https://github.com/asheroto/Generate-Certs/releases/latest/download/Generate-Certs.exe>) page, then open Generate-Certs.exe or run it from the command line.

### Linux

Install .NET 5 for Linux x64:
```
wget dot.net/v1/dotnet-install.sh -O dotnet.sh
bash dotnet.sh -c 5.0
```

Run Generate-Certs:
```
wget https://github.com/asheroto/Generate-Certs/releases/latest/download/Generate-Certs
chmod +x Generate-Certs
./Generate-Certs
```

# Screenshots

## Info/Warning
![Generate-Certs Initial Screen](https://github.com/asheroto/Generate-Certs/blob/master/screenshots/1.png)

## Configuration
![Generate-Certs Configuration](https://github.com/asheroto/Generate-Certs/blob/master/screenshots/2.png)

## Final Result
![Final Result](https://github.com/asheroto/Generate-Certs/blob/master/screenshots/3.png)