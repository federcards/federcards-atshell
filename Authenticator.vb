
Option Explicit On
Option Strict On

Imports System
Imports System.Security.Cryptography


Imports BasicCard
Imports BasicCard.BasicCards
Imports BasicCard.Framework
Imports BasicCard.GenericCards




Public Class Authenticator

    Private sharedsecret(31) As Byte
    Private k(31) As Byte

    Public encryption_key(31) As Byte

    Public Sub New(ByVal sharedsecret As BasicCardString)
        MyBase.New
        Dim i As Byte
        For i = 0 To 31
            If i >= sharedsecret.Length Then
                Me.sharedsecret(i) = 0
            Else
                Me.sharedsecret(i) = sharedsecret(i)
            End If
        Next

        Me.k = Me.calc_sha256(Me.sharedsecret)
    End Sub

    Private Function calc_sha256(ByVal bytes() As Byte) As Byte()
        Dim hasher As SHA256 = SHA256.create()
        calc_sha256 = hasher.ComputeHash(bytes)
    End Function

    Private Function calc_sha1(ByVal bytes() As Byte) As Byte()
        Dim hasher As SHA1 = SHA1.create()
        calc_sha1 = hasher.ComputeHash(bytes)
    End Function

    Private Function get_aes() As AesManaged
        Dim zeros() As Byte = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        get_aes = New AesManaged()
        get_aes.Mode = CipherMode.ECB
        get_aes.IV = zeros
        get_aes.Key = Me.k
        get_aes.Padding = PaddingMode.None
    End Function

    Private Function aes_encrypt(ByVal bytes() As Byte) As Byte()
        Dim output_buffer(15) As Byte
        Dim aesalg As AesManaged = Me.get_aes()
        Dim encryptor As ICryptoTransform
        encryptor = aesalg.CreateEncryptor(aesalg.Key, aesalg.IV)
        encryptor.TransformBlock(bytes, 0, bytes.length, output_buffer, 0)
        aes_encrypt = output_buffer
    End Function

    Private Function aes_decrypt(ByVal bytes() As Byte) As Byte()
        Dim output_buffer(15) As Byte
        Dim aesalg As AesManaged = Me.get_aes()
        Dim decryptor As ICryptoTransform
        decryptor = aesalg.CreateDecryptor(aesalg.Key, aesalg.IV)
        decryptor.TransformBlock(bytes, 0, bytes.length, output_buffer, 0)
        aes_decrypt = output_buffer
    End Function


    Public Function answer(ByVal challenge As BasicCardString) As BasicCardString
        Dim answer_bytes(16 + 6 - 1) As Byte
        Dim i As Integer
        Dim session_secret() As Byte

        answer_bytes = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, &H61, &H6E, &H73, &H77, &H65, &H72}
        session_secret = aes_decrypt(challenge.Characters)
        For i = 0 To 15
            answer_bytes(i) = session_secret(i)
        Next

        Me.encryption_key = calc_sha256(session_secret)

        Dim sha1answer_full() As Byte
        Dim sha1answer_16(15) As Byte
        sha1answer_full = Me.calc_sha1(answer_bytes)
        For i = 0 To 15
            sha1answer_16(i) = sha1answer_full(i)
        Next
        answer = New BasicCardString(aes_encrypt(sha1answer_16))
        Console.Writeline("ANSWER=" + HexStr.bstr2hex(answer))


    End Function

End Class
