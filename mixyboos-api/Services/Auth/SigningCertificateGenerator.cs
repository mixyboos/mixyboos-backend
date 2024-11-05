using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace MixyBoos.Api.Services.Auth;

public static class SigningCertificateGenerator {
    public static X509Certificate2 CreateEncryptionCertificate() {
        if (File.Exists("encryption-certificate.pfx")) {
            return new X509Certificate2("encryption-certificate.pfx");
        }

        using var algorithm = RSA.Create(keySizeInBits: 2048);

        var subject = new X500DistinguishedName("CN=Mixboos Encryption Certificate");
        var request =
            new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment,
            critical: true));

        var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2));

        File.WriteAllBytes("encryption-certificate.pfx", certificate.Export(X509ContentType.Pfx, string.Empty));

        return new X509Certificate2("encryption-certificate.pfx");
    }

    public static X509Certificate2 CreateSigningCertificate() {
        if (File.Exists("signing-certificate.pfx")) {
            return new X509Certificate2("signing-certificate.pfx");
        }

        using var algorithm = RSA.Create(keySizeInBits: 2048);

        var subject = new X500DistinguishedName("CN=Mixboos Encryption Certificate");
        var request =
            new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature,
            critical: true));

        var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2));

        File.WriteAllBytes("signing-certificate.pfx", certificate.Export(X509ContentType.Pfx, string.Empty));

        return new X509Certificate2("signing-certificate.pfx");
    }
}
