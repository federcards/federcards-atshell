
Option Explicit On
Option Strict On

Imports System
Imports BasicCard
Imports BasicCard.BasicCards
Imports BasicCard.Framework
Imports BasicCard.GenericCards




Public Class CardTerminal
    Inherits AbstractBasicCardService

    Shared Sub New()
    End Sub

    Public Function get_card_status() As BasicCardResponse
        Dim ret As BasicCardCommand
        ret = New BasicCardCommand(&H88, &H0)
        ret.LE = 1

        get_card_status = DoCommandAndResponse(ret)
        get_card_status.CheckSW1SW2()
    End Function

    Public Function do_login() As BasicCardResponse
        ' get challenge, ask for user answer, and forward to card
        Dim cmd As BasicCardCommand
        Dim ret As BasicCardResponse
        Dim challenge As BasicCardString
        Dim user_password As BasicCardString

        ' fetch current challenge string
        cmd = New BasicCardCommand(&H88, &H2)
        cmd.LE = 0
        ret = DoCommandAndResponse(cmd)
        challenge = ret.FetchBasicString()
        System.Console.WriteLine("CHALLENGE=" + HexStr.bstr2hex(challenge))

        ' Fetch user password and generate an answer
        Do
            System.Console.WriteLine("PASSWORD_HEX")
            user_password = HexStr.hex2bstr(System.Console.ReadLine())
        Loop While user_password.Length < 1

        Dim auth As Authenticator = New Authenticator(user_password)

        cmd = New BasicCardCommand(&H88, &H4)
        cmd.AppendBasicString(auth.answer(challenge))
        cmd.LE = 1
        ret = DoCommandAndResponse(cmd)

        ' Record challenge secret, which will be used for encrypted session.
        Me.SetKey(1, auth.encryption_key)

        do_login = ret
    End Function

    Public Sub start()
        Dim status As BasicCardResponse = Me.get_card_status()
        Dim remaining_attempts As Byte = status.FetchBasicByte()

        While remaining_attempts <> &HFF
            ' login required
            System.Console.WriteLine("'REMAINING_ATTEMPTS=" + remaining_attempts.toString())
            status = Me.do_login()
            remaining_attempts = status.FetchBasicByte()

            If remaining_attempts = 0 Then
                System.Console.WriteLine("CARD_LOCKED")
                Exit Sub
            End If
        End While

        ' Login successful.
        Me.CmdProEncryption(1, CryptAlgo.Aes128)
        Me.at_start()
    End Sub

    Public Sub at_start()
        Dim cmd As BasicCardCommand
        Dim ret As BasicCardResponse
        Dim retstr As BasicCardString

        cmd = New BasicCardCommand(&H88, &H6)
        cmd.AppendBasicString(
            New BasicCardString("AT+STATUS", BasicCardString.CharsetIdAscii))
        ret = DoCommandAndResponse(cmd)
        ret.CheckSW1SW2()

        retstr = ret.FetchBasicString()
        console.WriteLine(retstr.ToString("ascii"))

    End Sub

End Class
