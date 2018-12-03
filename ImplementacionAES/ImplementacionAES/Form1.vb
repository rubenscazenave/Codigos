Imports System.Security.Cryptography
Imports System.Text
Imports System.IO

Public Class Form1

    Dim Salt As String = "560A18CD-6346-4CF0-A2E8-671F9B6B9EA9"
    Dim PassWord As String = "xjfhweiukdskjfndpqr87Oo048cbacrtlpgupspr"

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Try
            Dim original As String = "Here is some data to encrypt!"

            ' Crea una nueva instancia de la clase, y se crea una llave e inicio vector mediante la funcion NewRijndaelObject
            'Using myRijndael = Me.NewRijndaelObject(Me.PassWord, Me.Salt)
            Using myRijndael = Me.NewRijndaelObject(Me.Salt)

                Me.TextBox1.Text = original
                ' Encripta el texto y lo asigna a una cadena de bytes()
                Dim encrypted As Byte() = EncryptStringToBytes(original, myRijndael.Key, myRijndael.IV)

                ' Desencripta la cadena de bytes a un texto.
                Dim roundtrip As String = DecryptStringFromBytes(encrypted, myRijndael.Key, myRijndael.IV)

                'Muestra el dato original y luego desencriptado
                Me.TextBox2.Text = "Original:  " & original
                Me.TextBox2.AppendText(Environment.NewLine & "Round Trip: " & roundtrip)
            End Using
        Catch ex As Exception
            Me.TextBox2.Text = ex.Message
        End Try

    End Sub


    Shared Function EncryptStringToBytes(ByVal plainText As String, ByVal Key() As Byte, ByVal IV() As Byte) As Byte()
        ' Valida los argumentos
        If plainText Is Nothing OrElse plainText.Length <= 0 Then
            Throw New ArgumentNullException("plainText")
        End If
        If Key Is Nothing OrElse Key.Length <= 0 Then
            Throw New ArgumentNullException("Key")
        End If
        If IV Is Nothing OrElse IV.Length <= 0 Then
            Throw New ArgumentNullException("Key")
        End If
        Dim encrypted() As Byte
        Dim outStr As String = String.Empty

        ' Crea una instancia del objeto con la llave e IV especificada.
        Using rijAlg = Rijndael.Create()
            rijAlg.Key = Key
            rijAlg.IV = IV
            ' Crea el objeto para ejecutar el cifrado
            Dim encryptor As ICryptoTransform = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV)
            ' Crea el stream usado para el cifrado
            Using msEncrypt As New MemoryStream()
                Using csEncrypt As New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)
                    Using swEncrypt As New StreamWriter(csEncrypt)
                        'escribe toda la data en la cadena
                        swEncrypt.Write(plainText)
                    End Using
                    encrypted = msEncrypt.ToArray()
                    'outStr = Convert.ToBase64String(msEncrypt.ToArray())
                End Using
            End Using
        End Using

        ' Retorna los bytes cifrados desde el stream en memoria.
        Return encrypted
    End Function 'EncryptStringToBytes


    Shared Function DecryptStringFromBytes(ByVal cipherText() As Byte, ByVal Key() As Byte, ByVal IV() As Byte) As String
        ' Valida los argumentos.
        If cipherText Is Nothing OrElse cipherText.Length <= 0 Then
            Throw New ArgumentNullException("cipherText")
        End If
        If Key Is Nothing OrElse Key.Length <= 0 Then
            Throw New ArgumentNullException("Key")
        End If
        If IV Is Nothing OrElse IV.Length <= 0 Then
            Throw New ArgumentNullException("Key")
        End If
        ' Declara el String usado para almacenar el texto desencriptado.
        Dim plaintext As String = Nothing

        ' Crea una instancia del objeto con la llave e IV especificada.
        Using rijAlg = Rijndael.Create()
            rijAlg.Key = Key
            rijAlg.IV = IV

            ' Crea un objeto decryptor del tipo ICryptoTransform para la transformación del stream.
            Dim decryptor As ICryptoTransform = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV)

            ' Crea el stream usado para el descifrado
            Using msDecrypt As New MemoryStream(cipherText)

                Using csDecrypt As New CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)

                    Using srDecrypt As New StreamReader(csDecrypt)

                        ' Lee los bytes descifrados del stream y los asigna a una variable string.
                        plaintext = srDecrypt.ReadToEnd()
                    End Using
                End Using
            End Using
        End Using

        Return plaintext
    End Function 'DecryptStringFromBytes


    'Private Function NewRijndaelObject(ByVal password As String, ByVal salt As String) As Rijndael
    Private Function NewRijndaelObject(ByVal salt As String) As Rijndael
        If password Is Nothing OrElse password.Length <= 0 Then
            Throw New ArgumentNullException("Password")
        End If
        If salt Is Nothing OrElse salt.Length <= 0 Then
            Throw New ArgumentNullException("Salt")
        End If
        Dim saltBytes As Byte() = Encoding.ASCII.GetBytes(salt)
        'Dim key = New Rfc2898DeriveBytes(password, saltBytes)
        Dim key = New Rfc2898DeriveBytes(password, 32)

        Dim aesAlg As Rijndael = Rijndael.Create()
        aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8)
        aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8)

        Return aesAlg
    End Function


End Class
