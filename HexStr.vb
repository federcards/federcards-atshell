
Option Explicit On

Imports System
Imports Microsoft.VisualBasic.Strings
Imports BasicCard.BasicCards



Public Class HexStr


    Public Shared Function bstr2hex(ByVal bstr As BasicCardString) As String
        Dim HEX_ALPHABET As String = "0123456789ABCDEF"
        Dim i As Integer
        bstr2hex = ""
        For i = 0 To bstr.characters.length - 1
            bstr2hex += HEX_ALPHABET((bstr.characters(i) And &HF0) >> 4) 
            bstr2hex += HEX_ALPHABET(bstr.characters(i) And &H0F)
        Next
    End Function


    Public Shared Function hex2bstr(ByVal hexInput As String) As BasicCardString
        Dim characters() As Byte
        Dim i As Integer
        Dim j As Integer = 0

        If hexInput.length Mod 2 = 1 Then
            ReDim characters(0)
        Else
            ReDim characters(hexInput.length / 2)
        End If

        For i = 1 To hexInput.length Step 2
            characters(j) = Convert.toUint16(Mid(hexInput, i, 2), 16)
            j += 1
        Next
        hex2bstr = New BasicCardString(characters)
    End Function

End Class
