Imports System.Data
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.IO
Imports System.Windows.Forms
Imports System.Linq


Public Class frmProcesarPedido

    Public Class ProductoPedido
        Public Property CodProd As String
        Public Property CantidadPedida As Decimal
    End Class

    Public Class LoteDisponible
        Public Property CodProd As String
        Public Property NroUnico As Integer
        Public Property NroLote As String
        Public Property CodUbic As String
        Public Property CantidadTomada As Decimal
        Public Property FechaV As Date
        Public Property FechaL As Date
        Public Property Precio2 As Decimal
        Public Property Costo As Decimal
    End Class

    Public Class DetalleItemFac
        Public Property NroLinea As Integer
        Public Property Producto As ProductoPedido
        Public Property Lote As LoteDisponible
        Public Property Descrip1 As String
        Public Property Descrip2 As String
        Public Property Refere As String
    End Class

    Public Class ClienteInfo
        Public Property CodClie As String
        Public Property Descrip As String
        Public Property Direc1 As String
        Public Property Direc2 As String
        Public Property ID3 As String
        Public Property CodZona As String
        Public Property NombreZona As String ' Nuevo campo para el nombre de la zona
        Public Property CodVend As String
        Public Property NombreVend As String ' Nuevo campo para el nombre de la zona
    End Class

    Private connectionString As String = "Data Source=SV-NAUTICA-A;Initial Catalog=NauticaAdminDb;User ID=sa;Password=Ay3y39y6;TrustServerCertificate=True;MultipleActiveResultSets=True"
    Private productosPedido As New List(Of ProductoPedido)
    Private clienteSeleccionado As ClienteInfo

    Private Sub frmProcesarPedido_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CargarClientes()
        ConfigurarGrid()
    End Sub
    ''CREAR PROCEDIMIENTO PARA VERIFICAR EL ARCHIVO DE EXCEL PARA SABER SI ESTA EXCEDIENDO EL LIMITE DE CREDITO POR EL PEDIDO
    ''SE VAN A VERIFICAR LOS CAMPOS DE: SALDO, LIMITE DE CREDITO Y TOTAL DE PEDIDO.
    ''AGREGAR CAMPO DE ZONA AL PROCESADOR DE PEDIDO
    ''MOSTRAR EL VENDEDOR ASIGNADO AL CLIENTE
    ''


    Private Sub CargarClientes()
        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Using cmd As New SqlCommand("SELECT CodClie, Descrip, Direc1, Direc2, ID3 FROM saclie ORDER BY Descrip", conn)
                    Dim da As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable
                    da.Fill(dt)

                    cboClientes.DataSource = dt
                    cboClientes.DisplayMember = "Descrip"
                    cboClientes.ValueMember = "CodClie"
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error al cargar clientes: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ConfigurarGrid()
        dgvProductos.AutoGenerateColumns = False
        dgvProductos.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = "CodProd",
            .HeaderText = "Código",
            .Width = 100
        })
        dgvProductos.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = "Descrip1",
            .HeaderText = "Descripción",
            .Width = 200
        })
        dgvProductos.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = "CantidadPedida",
            .HeaderText = "Cantidad",
            .Width = 80
        })
        dgvProductos.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = "Precio",
            .HeaderText = "Precio",
            .Width = 80
        })
        dgvProductos.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = "Total",
            .HeaderText = "Total",
            .Width = 80
        })
    End Sub

    Private Sub btnSeleccionarExcel_Click(sender As Object, e As EventArgs) Handles btnSeleccionarExcel.Click
        Dim openFileDialog As New OpenFileDialog() With {
            .Filter = "Excel Files|*.xlsx;*.xls",
            .Title = "Seleccionar archivo Excel"
        }

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            Try
                productosPedido = LeerExcel(openFileDialog.FileName)
                MostrarProductosEnGrid()
            Catch ex As Exception
                MessageBox.Show("Error al leer el archivo Excel: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Function LeerExcel(ruta As String) As List(Of ProductoPedido)
        Dim lista As New List(Of ProductoPedido)
        Dim connStr = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={ruta};Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;'"

        Using conn As New OleDbConnection(connStr)
            conn.Open()
            Dim dtSchema As DataTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)
            Dim sheetName As String = dtSchema.Rows(0)("TABLE_NAME").ToString()

            ' Consulta para obtener los nombres de columnas
            Dim queryColumnas = $"SELECT TOP 1 * FROM [{sheetName}]"
            Using cmdColumnas As New OleDbCommand(queryColumnas, conn)
                Using rdrColumnas = cmdColumnas.ExecuteReader()
                    If rdrColumnas.Read() Then
                        ' Crear diccionario de columnas (forma compatible con VB.NET)
                        Dim columnasDisponibles As New Dictionary(Of String, Integer)
                        For i As Integer = 0 To rdrColumnas.FieldCount - 1
                            columnasDisponibles.Add(rdrColumnas.GetName(i), i)
                        Next

                        ' Verificar columnas requeridas
                        Dim tieneCodProd As Boolean = columnasDisponibles.Keys.Any(
                        Function(k) k.Equals("CodProd", StringComparison.OrdinalIgnoreCase) OrElse
                                   k.Contains("Código"))

                        Dim tieneCantidad As Boolean = columnasDisponibles.Keys.Any(
                        Function(k) k.Equals("Cantidad", StringComparison.OrdinalIgnoreCase) OrElse
                                   k.Contains("Cant"))

                        If Not tieneCodProd OrElse Not tieneCantidad Then
                            Throw New Exception("El archivo Excel no tiene las columnas requeridas (CodProd y Cantidad)")
                        End If

                        ' Leer datos
                        Using cmdDatos As New OleDbCommand($"SELECT * FROM [{sheetName}]", conn)
                            Using rdr = cmdDatos.ExecuteReader()
                                While rdr.Read()
                                    Try
                                        ' Obtener CodProd
                                        Dim colCodProd = columnasDisponibles.Keys.FirstOrDefault(
                                        Function(k) k.Equals("CodProd", StringComparison.OrdinalIgnoreCase) OrElse
                                                   k.Contains("Código"))
                                        Dim codProd As String = rdr(colCodProd).ToString().Trim()

                                        ' Obtener Cantidad
                                        Dim colCantidad = columnasDisponibles.Keys.FirstOrDefault(
                                        Function(k) k.Equals("Cantidad", StringComparison.OrdinalIgnoreCase) OrElse
                                                   k.Contains("Cant"))
                                        Dim cantidad As Decimal = 0
                                        Decimal.TryParse(rdr(colCantidad).ToString(), cantidad)

                                        If Not String.IsNullOrEmpty(codProd) AndAlso cantidad > 0 Then
                                            lista.Add(New ProductoPedido With {
                                            .CodProd = codProd,
                                            .CantidadPedida = cantidad
                                        })
                                        End If
                                    Catch ex As Exception
                                        Console.WriteLine($"Error procesando fila: {ex.Message}")
                                    End Try
                                End While
                            End Using
                        End Using
                    End If
                End Using
            End Using
        End Using

        Return lista
    End Function

    Private Sub MostrarProductosEnGrid()
        If productosPedido Is Nothing OrElse productosPedido.Count = 0 Then
            MessageBox.Show("No se encontraron productos en el archivo Excel", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If ' <-- Aquí cerramos el If, no un Try

        Dim dt As New DataTable
        dt.Columns.Add("CodProd", GetType(String))
        dt.Columns.Add("Descrip1", GetType(String))
        dt.Columns.Add("CantidadPedida", GetType(Decimal))
        dt.Columns.Add("Precio", GetType(Decimal))
        dt.Columns.Add("Total", GetType(Decimal))

        Using conn As New SqlConnection(connectionString)
            Try
                conn.Open()
                For Each prod In productosPedido
                    Dim precio = ObtenerPrecioProducto(conn, prod.CodProd)
                    Dim descrip = ObtenerDescripcionProducto(conn, prod.CodProd)
                    dt.Rows.Add(prod.CodProd, descrip, prod.CantidadPedida, precio, prod.CantidadPedida * precio)
                Next
            Catch ex As Exception
                MessageBox.Show("Error al obtener datos de productos: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try
        End Using

        dgvProductos.DataSource = dt
        CalcularTotales()
    End Sub

    Private Function ObtenerPrecioProducto(conn As SqlConnection, codProd As String) As Decimal
        Using cmd As New SqlCommand("SELECT Precio2 FROM salote WHERE CodProd = @CodProd", conn)
            cmd.Parameters.AddWithValue("@CodProd", codProd)
            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing Then Return Convert.ToDecimal(result)
        End Using
        Return 0
    End Function

    Private Function ObtenerDescripcionProducto(conn As SqlConnection, codProd As String) As String
        Using cmd As New SqlCommand("SELECT Descrip FROM saprod WHERE CodProd = @CodProd", conn)
            cmd.Parameters.AddWithValue("@CodProd", codProd)
            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing Then Return result.ToString()
        End Using
        Return ""
    End Function

    Private Sub CalcularTotales()
        Dim total As Decimal = 0
        For Each row As DataGridViewRow In dgvProductos.Rows
            If Not row.IsNewRow Then
                total += Convert.ToDecimal(row.Cells("Total").Value)
            End If
        Next
        lblTotal.Text = total.ToString("C2")
    End Sub

    Private Sub cboClientes_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboClientes.SelectedIndexChanged
        If cboClientes.SelectedItem IsNot Nothing AndAlso TypeOf cboClientes.SelectedItem Is DataRowView Then
            Dim row As DataRowView = DirectCast(cboClientes.SelectedItem, DataRowView)
            Dim codClie = row("CodClie").ToString()

            Using conn As New SqlConnection(connectionString)
                conn.Open()
                clienteSeleccionado = ObtenerDatosCliente(conn, codClie)
            End Using

            MostrarDatosCliente()
        End If
    End Sub

    Private Sub MostrarDatosCliente()
        If clienteSeleccionado IsNot Nothing Then
            txtCodCliente.Text = clienteSeleccionado.CodClie
            txtNombreCliente.Text = clienteSeleccionado.Descrip
            txtDireccion1.Text = clienteSeleccionado.Direc1
            txtDireccion2.Text = clienteSeleccionado.Direc2
            txtIdentificacion.Text = clienteSeleccionado.ID3
            TxtZona.Text = clienteSeleccionado.NombreZona
            TxtVend.Text = clienteSeleccionado.NombreVend
        End If
    End Sub

    Private Sub btnProcesar_Click(sender As Object, e As EventArgs) Handles btnProcesar.Click
        If clienteSeleccionado Is Nothing Then
            MessageBox.Show("Debe seleccionar un cliente", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If productosPedido.Count = 0 Then
            MessageBox.Show("No hay productos para procesar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Cursor = Cursors.WaitCursor
            ProcesarPedido()
            Cursor = Cursors.Default
            MessageBox.Show("Pedido procesado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            Cursor = Cursors.Default
            MessageBox.Show("Error al procesar pedido: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ProcesarPedido()
        Using conn As New SqlConnection(connectionString)
            conn.Open()

            ' Obtener número de documento
            Dim numeroDocumento = ObtenerProximoNumero(conn, "00000", Nothing, "PRXFACTESP")
            Dim nroLineaGlobal As Integer = 1
            Dim itemsAFacturar As New List(Of DetalleItemFac)
            Dim productosFaltantes As New List(Of String)

            ' Procesar cada producto
            For Each prod In productosPedido
                Dim lotes As List(Of LoteDisponible) = ObtenerLotesDisponibles(conn, prod.CodProd, prod.CantidadPedida, "01")

                If lotes Is Nothing OrElse lotes.Count = 0 Then
                    productosFaltantes.Add($"{prod.CodProd} no existe o no tiene existencia suficiente.")
                    Continue For
                End If

                Dim cantidadAsignada As Decimal = 0
                For Each lote In lotes
                    Dim productoInfo = ObtenerDatosProducto(conn, prod.CodProd)
                    If productoInfo Is Nothing Then
                        productosFaltantes.Add($"{prod.CodProd} no fue encontrado en SAPROD.")
                        Exit For
                    End If

                    itemsAFacturar.Add(New DetalleItemFac With {
                        .NroLinea = nroLineaGlobal,
                        .Producto = prod,
                        .Lote = lote,
                        .Descrip1 = productoInfo.Item1,
                        .Descrip2 = productoInfo.Item2,
                        .Refere = productoInfo.Item3
                    })

                    cantidadAsignada += lote.CantidadTomada
                    nroLineaGlobal += 1
                Next

                If cantidadAsignada < prod.CantidadPedida Then
                    productosFaltantes.Add($"Al código {prod.CodProd} le faltaron {prod.CantidadPedida - cantidadAsignada} unidades.")
                End If
            Next

            ' Insertar items
            For Each item In itemsAFacturar
                InsertarEnSaitemfac(conn, numeroDocumento, item)
            Next

            ' Insertar encabezado
            InsertarEnSafact(conn, numeroDocumento, itemsAFacturar, clienteSeleccionado.CodClie, "01", "01")
            ' Registrar productos con faltantes en SABACKORDER
            For Each prod In productosPedido
                ' Calcular cantidad asignada
                Dim cantidadAsignada As Decimal = itemsAFacturar.
        Where(Function(i) i.Producto.CodProd = prod.CodProd).
        Sum(Function(i) i.Lote.CantidadTomada)

                Dim cantidadFaltante As Decimal = prod.CantidadPedida - cantidadAsignada

                If cantidadFaltante > 0 Then
                    ' Tomar el último lote asignado (si lo hubo) o usar valores por defecto
                    Dim ultimoLote As LoteDisponible = itemsAFacturar.
            Where(Function(i) i.Producto.CodProd = prod.CodProd).
            Select(Function(i) i.Lote).LastOrDefault()

                    Dim nroUnico = If(ultimoLote IsNot Nothing, ultimoLote.NroUnico, 0)
                    Dim nroLote = If(ultimoLote IsNot Nothing, ultimoLote.NroLote, "NO DISPONIBLE")

                    Using cmd As New SqlCommand("INSERT INTO SABACKORDER 
            (NumeroD, FechaPedido, CodProd, NroUnico, NroLote, CantidadPedida, CantidadFaltante, CodClie)
            VALUES (@NumeroD, @FechaPedido, @CodProd, @NroUnico, @NroLote, @CantidadPedida, @CantidadFaltante, @CodClie)", conn)

                        cmd.Parameters.AddWithValue("@NumeroD", numeroDocumento)
                        cmd.Parameters.AddWithValue("@FechaPedido", DateTime.Now)
                        cmd.Parameters.AddWithValue("@CodProd", prod.CodProd)
                        cmd.Parameters.AddWithValue("@NroUnico", nroUnico)
                        cmd.Parameters.AddWithValue("@NroLote", nroLote)
                        cmd.Parameters.AddWithValue("@CantidadPedida", prod.CantidadPedida)
                        cmd.Parameters.AddWithValue("@CantidadFaltante", cantidadFaltante)
                        cmd.Parameters.AddWithValue("@CodClie", clienteSeleccionado.CodClie)

                        cmd.ExecuteNonQuery()
                    End Using
                End If
            Next


            ' Mostrar resultados
            Dim mensaje = $"Documento generado: {numeroDocumento}"
            If productosFaltantes.Count > 0 Then
                mensaje += vbCrLf & vbCrLf & "Advertencias:" & vbCrLf & String.Join(vbCrLf, productosFaltantes)
            End If

            MessageBox.Show(mensaje, "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information)
            If productosFaltantes.Count > 0 Then
                Dim rutaArchivo As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Faltantes_" & numeroDocumento & "_" & DateTime.Now.ToString("ddMMyyy") & ".txt")
                File.WriteAllLines(rutaArchivo, productosFaltantes)
            End If

        End Using
    End Sub

    Sub InsertarEnSaitemfac(conn As SqlConnection, numeroD As String, item As DetalleItemFac)
        Using cmd As New SqlCommand("INSERT INTO saitemfac (Codsucu, TipoFac, NumeroD, OTipo, ONumero, ONroLinea, ONroLineaC, NumeroE, NroLinea, NroLineaC, CodItem, CodUbic, CodUsua, CODAUTH, CODMECA, CODVEND, Descrip1, Descrip2, Descrip3, Descrip4, Descrip5, Descrip6, Descrip7, Descrip8, Descrip9, Descrip10, Refere, Signo, CantMayor, Cantidad, CantidadD, CantidadT, CantidadO, CantidadA, CantidadU, CantidadUA, ExistAntU, ExistAnt, Tara, Factor, TotalItem, Costo, TipoPVP, Precio, PrecioI, MtoTax, MtoTaxO, PriceO, Descto, NroUnicoL, NroLote, FechaE, FechaL, FechaV, EsServ, Esunid, EsFreeP, EsPesa, UsaServ, DesSeri, Descomp, TIpodata, EsExento, DesLote, CantidadOriginal) VALUES (@Codsucu, @TipoFac, @NumeroD, NULL, NULL, 0, 0, NULL, @NroLinea, 0, @CodItem, @CodUbic, NULL, NULL, NULL, '01', @Descrip1, @Descrip2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, @Refere, 1, 1, @Cantidad, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, @TotalItem, @Costo, 0, @Precio, 0, 0, 0, @Precio, 0, @NroUnicoL, @NroLote, GETDATE(), @FechaL, @FechaV, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, NULL)", conn)

            cmd.Parameters.AddWithValue("@Codsucu", "00000")
            cmd.Parameters.AddWithValue("@TipoFac", "E")
            cmd.Parameters.AddWithValue("@NumeroD", numeroD)
            cmd.Parameters.AddWithValue("@NroLinea", item.NroLinea)
            cmd.Parameters.AddWithValue("@CodItem", item.Producto.CodProd)
            cmd.Parameters.AddWithValue("@CodUbic", item.Lote.CodUbic)
            cmd.Parameters.AddWithValue("@Descrip1", item.Descrip1)
            cmd.Parameters.AddWithValue("@Descrip2", item.Descrip2)
            cmd.Parameters.AddWithValue("@Refere", item.Refere)
            cmd.Parameters.AddWithValue("@Cantidad", item.Lote.CantidadTomada)
            cmd.Parameters.AddWithValue("@TotalItem", item.Lote.CantidadTomada * item.Lote.Precio2)
            cmd.Parameters.AddWithValue("@Costo", item.Lote.Costo)
            cmd.Parameters.AddWithValue("@Precio", item.Lote.Precio2)
            cmd.Parameters.AddWithValue("@PriceO", item.Lote.Precio2)
            cmd.Parameters.AddWithValue("@NroUnicoL", item.Lote.NroUnico)
            cmd.Parameters.AddWithValue("@NroLote", item.Lote.NroLote)
            cmd.Parameters.AddWithValue("@FechaL", item.Lote.FechaL)
            cmd.Parameters.AddWithValue("@FechaV", item.Lote.FechaV)

            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Function ObtenerDatosCliente(conn As SqlConnection, codClie As String) As ClienteInfo
        Dim cliente As New ClienteInfo()

        ' Consulta para obtener datos básicos del cliente
        Using cmd As New SqlCommand("SELECT CodClie, Descrip, Direc1, Direc2, ID3, CodZona, CodVend FROM saclie WHERE CodClie = @CodClie", conn)
            cmd.Parameters.AddWithValue("@CodClie", codClie)
            Using rdr = cmd.ExecuteReader()
                If rdr.Read() Then
                    cliente.CodClie = rdr("CodClie").ToString()
                    cliente.Descrip = rdr("Descrip").ToString()
                    cliente.Direc1 = rdr("Direc1").ToString()
                    cliente.Direc2 = rdr("Direc2").ToString()
                    cliente.ID3 = rdr("ID3").ToString()
                    cliente.CodZona = rdr("CodZona").ToString()
                    cliente.CodVend = rdr("CodVend").ToString()
                End If
            End Using
        End Using

        ' Consulta para obtener el nombre de la zona
        If Not String.IsNullOrEmpty(cliente.CodZona) Then
            Using cmdZona As New SqlCommand("SELECT Descrip FROM sazona WHERE CodZona = @CodZona", conn)
                cmdZona.Parameters.AddWithValue("@CodZona", cliente.CodZona)
                Dim result = cmdZona.ExecuteScalar()
                If result IsNot Nothing Then
                    cliente.NombreZona = result.ToString()
                End If
            End Using
        End If
        If Not String.IsNullOrEmpty(cliente.CodVend) Then
            Using cmdZona As New SqlCommand("SELECT Descrip FROM SAVEND WHERE CodVend = @CodVend", conn)
                cmdZona.Parameters.AddWithValue("@CodVend", cliente.CodVend)
                Dim result = cmdZona.ExecuteScalar()
                If result IsNot Nothing Then
                    cliente.NombreVend = result.ToString()
                End If
            End Using
        End If

        Return cliente
    End Function

    Function ObtenerProximoNumero(conn As SqlConnection, codSucu As String, codEsta As String, prxField As String) As String
        Using cmd As New SqlCommand("SP_ADM_PROXCORREL", conn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@CODSUCU", codSucu)
            cmd.Parameters.AddWithValue("@CODESTA", If(codEsta, DBNull.Value))
            cmd.Parameters.AddWithValue("@PRXFIELD", prxField)
            Dim paramOut = cmd.Parameters.Add("@NUMERO", SqlDbType.VarChar, 25)
            paramOut.Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            Return paramOut.Value.ToString()
        End Using
    End Function

    Function ObtenerLotesDisponibles(conn As SqlConnection, codProd As String, cantidad As Decimal, codUbic As String) As List(Of LoteDisponible)
        Dim lista As New List(Of LoteDisponible)
        Using cmd As New SqlCommand("ObtenerLotesParaAsignacion", conn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@CodProd", codProd)
            cmd.Parameters.AddWithValue("@CantidadDeseada", cantidad)
            cmd.Parameters.AddWithValue("@CodUbic", codUbic)
            Using rdr = cmd.ExecuteReader()
                While rdr.Read()
                    Dim nroUnicoActual As Integer = Convert.ToInt32(rdr("NroUnico"))
                    lista.Add(New LoteDisponible With {
                    .CodProd = rdr("CodProd").ToString(),
                    .NroUnico = nroUnicoActual,
                    .NroLote = rdr("NroLote").ToString(),
                    .CodUbic = rdr("CodUbic").ToString(),
                    .CantidadTomada = Convert.ToDecimal(rdr("CantidadTomada")),
                    .FechaV = Convert.ToDateTime(rdr("FechaV")),
                    .FechaL = Convert.ToDateTime(rdr("FechaV")),
                    .Precio2 = ObtenerPrecio2(conn, nroUnicoActual), ' Cambio clave aquí
                    .Costo = ObtenerCosto(conn, rdr("NroUnico").ToString())
                })
                End While
            End Using
        End Using
        Return lista
    End Function

    Function ObtenerDatosProducto(conn As SqlConnection, codProd As String) As Tuple(Of String, String, String)
        Using cmd As New SqlCommand("SELECT Descrip, Descrip2, Refere FROM saprod WHERE CodProd = @CodProd", conn)
            cmd.Parameters.AddWithValue("@CodProd", codProd)
            Using rdr = cmd.ExecuteReader()
                If rdr.Read() Then
                    Return Tuple.Create(rdr("Descrip").ToString(), rdr("Descrip2").ToString(), rdr("Refere").ToString())
                End If
            End Using
        End Using
        Return Nothing
    End Function

    Function ObtenerPrecio2(conn As SqlConnection, nroUnico As Integer) As Decimal
        Using cmd As New SqlCommand("SELECT Precio2 FROM salote WHERE NroUnico = @NroUnico", conn)
            cmd.Parameters.AddWithValue("@NroUnico", nroUnico)
            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                Return Convert.ToDecimal(result)
            End If
        End Using
        Return 0
    End Function

    Function ObtenerCosto(conn As SqlConnection, nroUnico As String) As Decimal
        Using cmd As New SqlCommand("SELECT Costo FROM salote WHERE NroUnico = @NroUnico", conn)
            cmd.Parameters.AddWithValue("@NroUnico", nroUnico)
            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing Then Return Convert.ToDecimal(result)
        End Using
        Return 0
    End Function

    Private Function ObtenerFactorConversion(conn As SqlConnection) As Decimal
        Using cmd As New SqlCommand("SELECT Factor FROM saconf", conn)
            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                Return Convert.ToDecimal(result)
            End If
        End Using
        Return 1D
    End Function



    Sub InsertarEnSafact(conn As SqlConnection, numeroD As String, items As List(Of DetalleItemFac), codClie As String, codVend As String, codUbic As String)
        ' Calcular totales
        Dim monto As Decimal = 0
        Dim costoPrd As Decimal = 0

        For Each item In items
            monto += item.Lote.CantidadTomada * item.Lote.Precio2
            costoPrd += item.Lote.CantidadTomada * item.Lote.Costo
        Next

        ' Obtener datos adicionales
        Dim factor = ObtenerFactorConversion(conn)
        Dim clienteInfo = ObtenerDatosCliente(conn, codClie)

        Using cmd As New SqlCommand(
            "INSERT INTO safact (
                CodSucu, TipoFac, NumeroD, NroTurno, NroCtrol, FromTran, EstadoFE, CodEsta, CodUsua, 
                CodTran, CodTarj, EsCorrel, CodConv, Signo, FechaT, TipoDev, OTipo, ONumero, 
                NumeroC, NumeroT, NumeroR, TipoTraE, AutSRI, NroEstable, PtoEmision, NumeroU, 
                NumeroF, NumeroNCF, NumeroP, NumeroE, NumeroZ, Moneda, Factor, MontoMEx, CodClie, 
                CodVend, CodUbic, Descrip, Direc1, Direc2, Direc3, ZipCode, Telef, ID3, Monto, 
                MtoTax, Fletes, TGravable, TGravable0, TExento, CostoPrd, CostoSrv, DesctoP, 
                RetenIVA, FechaR, FechaI, FechaE, FechaV, MtoTotal, Contado, Credito, CancelI, 
                CancelA, CancelE, CancelC, CancelT, CancelG, CancelP, Cambio, MtoExtra, ValorPtos, 
                Descto1, PctAnual, MtoInt1, Descto2, PctManejo, MtoInt2, SaldoAct, MtoPagos, 
                MtoNCredito, MtoNDebito, MtoFinanc, DetalChq, TotalPrd, TotalSrv, OrdenC, CodOper, 
                NGiros, NMeses, MtoComiVta, MtoComiCob, MtoComiVtaD, MtoComiCobD, ImpuestoD, 
                CancelTips, NroUnicoL, CodAlte, Parcial, Notas1, Notas2, Notas3, Notas4, Notas5, 
                Notas6, Notas7, Notas8, Notas9, Notas10, BaseImpuestD
            ) VALUES (
                @CodSucu, @TipoFac, @NumeroD, @NroTurno, @NroCtrol, @FromTran, @EstadoFE, @CodEsta, @CodUsua, 
                @CodTran, @CodTarj, @EsCorrel, @CodConv, @Signo, @FechaT, @TipoDev, @OTipo, @ONumero, 
                @NumeroC, @NumeroT, @NumeroR, @TipoTraE, @AutSRI, @NroEstable, @PtoEmision, @NumeroU, 
                @NumeroF, @NumeroNCF, @NumeroP, @NumeroE, @NumeroZ, @Moneda, @Factor, @MontoMEx, @CodClie, 
                @CodVend, @CodUbic, @Descrip, @Direc1, @Direc2, @Direc3, @ZipCode, @Telef, @ID3, @Monto, 
                @MtoTax, @Fletes, @TGravable, @TGravable0, @TExento, @CostoPrd, @CostoSrv, @DesctoP, 
                @RetenIVA, @FechaR, @FechaI, @FechaE, @FechaV, @MtoTotal, @Contado, @Credito, @CancelI, 
                @CancelA, @CancelE, @CancelC, @CancelT, @CancelG, @CancelP, @Cambio, @MtoExtra, @ValorPtos, 
                @Descto1, @PctAnual, @MtoInt1, @Descto2, @PctManejo, @MtoInt2, @SaldoAct, @MtoPagos, 
                @MtoNCredito, @MtoNDebito, @MtoFinanc, @DetalChq, @TotalPrd, @TotalSrv, @OrdenC, @CodOper, 
                @NGiros, @NMeses, @MtoComiVta, @MtoComiCob, @MtoComiVtaD, @MtoComiCobD, @ImpuestoD, 
                @CancelTips, @NroUnicoL, @CodAlte, @Parcial, @Notas1, @Notas2, @Notas3, @Notas4, @Notas5, 
                @Notas6, @Notas7, @Notas8, @Notas9, @Notas10, @BaseImpuestD
            )", conn)

            ' Parámetros básicos
            cmd.Parameters.AddWithValue("@CodSucu", "00000")
            cmd.Parameters.AddWithValue("@TipoFac", "E")
            cmd.Parameters.AddWithValue("@NumeroD", numeroD)
            cmd.Parameters.AddWithValue("@NroTurno", 0)
            cmd.Parameters.AddWithValue("@NroCtrol", "0")
            cmd.Parameters.AddWithValue("@FromTran", 0)
            cmd.Parameters.AddWithValue("@EstadoFE", 0)
            cmd.Parameters.AddWithValue("@CodEsta", "SV-NAUTICA-A")
            cmd.Parameters.AddWithValue("@CodUsua", "18981137")
            cmd.Parameters.AddWithValue("@CodTran", DBNull.Value)
            cmd.Parameters.AddWithValue("@CodTarj", DBNull.Value)
            cmd.Parameters.AddWithValue("@EsCorrel", 1)
            cmd.Parameters.AddWithValue("@CodConv", DBNull.Value)
            cmd.Parameters.AddWithValue("@Signo", 1)
            cmd.Parameters.AddWithValue("@FechaT", DateTime.Now)
            cmd.Parameters.AddWithValue("@TipoDev", 0)
            cmd.Parameters.AddWithValue("@OTipo", DBNull.Value)
            cmd.Parameters.AddWithValue("@ONumero", DBNull.Value)
            cmd.Parameters.AddWithValue("@NumeroC", DBNull.Value)
            cmd.Parameters.AddWithValue("@NumeroT", DBNull.Value)
            cmd.Parameters.AddWithValue("@NumeroR", DBNull.Value)
            cmd.Parameters.AddWithValue("@TipoTraE", 0)
            cmd.Parameters.AddWithValue("@AutSRI", DBNull.Value)
            cmd.Parameters.AddWithValue("@NroEstable", DBNull.Value)
            cmd.Parameters.AddWithValue("@PtoEmision", DBNull.Value)
            cmd.Parameters.AddWithValue("@NumeroU", DBNull.Value)
            cmd.Parameters.AddWithValue("@NumeroF", DBNull.Value)
            cmd.Parameters.AddWithValue("@NumeroNCF", DBNull.Value)
            cmd.Parameters.AddWithValue("@NumeroP", DBNull.Value)
            cmd.Parameters.AddWithValue("@NumeroE", DBNull.Value)
            cmd.Parameters.AddWithValue("@NumeroZ", DBNull.Value)
            cmd.Parameters.AddWithValue("@Moneda", DBNull.Value)
            cmd.Parameters.AddWithValue("@Factor", factor)
            cmd.Parameters.AddWithValue("@MontoMEx", 0)

            ' Datos del cliente
            cmd.Parameters.AddWithValue("@CodClie", codClie)
            cmd.Parameters.AddWithValue("@CodVend", codVend)
            cmd.Parameters.AddWithValue("@CodUbic", codUbic)
            cmd.Parameters.AddWithValue("@Descrip", clienteInfo.Descrip)
            cmd.Parameters.AddWithValue("@Direc1", clienteInfo.Direc1)
            cmd.Parameters.AddWithValue("@Direc2", clienteInfo.Direc2)
            cmd.Parameters.AddWithValue("@Direc3", DBNull.Value)
            cmd.Parameters.AddWithValue("@ZipCode", DBNull.Value)
            cmd.Parameters.AddWithValue("@Telef", DBNull.Value)
            cmd.Parameters.AddWithValue("@ID3", clienteInfo.ID3)
            cmd.Parameters.AddWithValue("@CodZona", clienteInfo.CodZona)
            cmd.Parameters.AddWithValue("@NombreVend", clienteInfo.NombreVend)

            ' Totales y montos
            cmd.Parameters.AddWithValue("@Monto", monto)
            cmd.Parameters.AddWithValue("@MtoTax", 0)
            cmd.Parameters.AddWithValue("@Fletes", 0)
            cmd.Parameters.AddWithValue("@TGravable", 0)
            cmd.Parameters.AddWithValue("@TGravable0", 0)
            cmd.Parameters.AddWithValue("@TExento", monto)
            cmd.Parameters.AddWithValue("@CostoPrd", costoPrd)
            cmd.Parameters.AddWithValue("@CostoSrv", 0)
            cmd.Parameters.AddWithValue("@DesctoP", 0)
            cmd.Parameters.AddWithValue("@RetenIVA", 0)
            cmd.Parameters.AddWithValue("@FechaR", DBNull.Value)
            cmd.Parameters.AddWithValue("@FechaI", DateTime.Now)
            cmd.Parameters.AddWithValue("@FechaE", DateTime.Now)
            cmd.Parameters.AddWithValue("@FechaV", DateTime.Now)
            cmd.Parameters.AddWithValue("@MtoTotal", monto)
            cmd.Parameters.AddWithValue("@Contado", 0)
            cmd.Parameters.AddWithValue("@Credito", monto)

            ' Valores en cero o nulos
            cmd.Parameters.AddWithValue("@CancelI", 0)
            cmd.Parameters.AddWithValue("@CancelA", 0)
            cmd.Parameters.AddWithValue("@CancelE", 0)
            cmd.Parameters.AddWithValue("@CancelC", 0)
            cmd.Parameters.AddWithValue("@CancelT", 0)
            cmd.Parameters.AddWithValue("@CancelG", 0)
            cmd.Parameters.AddWithValue("@CancelP", 0)
            cmd.Parameters.AddWithValue("@Cambio", 0)
            cmd.Parameters.AddWithValue("@MtoExtra", 0)
            cmd.Parameters.AddWithValue("@ValorPtos", 0)
            cmd.Parameters.AddWithValue("@Descto1", 0)
            cmd.Parameters.AddWithValue("@PctAnual", 0)
            cmd.Parameters.AddWithValue("@MtoInt1", 0)
            cmd.Parameters.AddWithValue("@Descto2", 0)
            cmd.Parameters.AddWithValue("@PctManejo", 0)
            cmd.Parameters.AddWithValue("@MtoInt2", 0)
            cmd.Parameters.AddWithValue("@SaldoAct", 0)
            cmd.Parameters.AddWithValue("@MtoPagos", 0)
            cmd.Parameters.AddWithValue("@MtoNCredito", 0)
            cmd.Parameters.AddWithValue("@MtoNDebito", 0)
            cmd.Parameters.AddWithValue("@MtoFinanc", 0)
            cmd.Parameters.AddWithValue("@DetalChq", DBNull.Value)
            cmd.Parameters.AddWithValue("@TotalPrd", monto)
            cmd.Parameters.AddWithValue("@TotalSrv", 0)
            cmd.Parameters.AddWithValue("@OrdenC", DBNull.Value)
            cmd.Parameters.AddWithValue("@CodOper", DBNull.Value)
            cmd.Parameters.AddWithValue("@NGiros", 0)
            cmd.Parameters.AddWithValue("@NMeses", 0)
            cmd.Parameters.AddWithValue("@MtoComiVta", 0)
            cmd.Parameters.AddWithValue("@MtoComiCob", 0)
            cmd.Parameters.AddWithValue("@MtoComiVtaD", 0)
            cmd.Parameters.AddWithValue("@MtoComiCobD", 0)
            cmd.Parameters.AddWithValue("@ImpuestoD", 0)
            cmd.Parameters.AddWithValue("@CancelTips", 0)
            cmd.Parameters.AddWithValue("@NroUnicoL", 0)
            cmd.Parameters.AddWithValue("@CodAlte", DBNull.Value)
            cmd.Parameters.AddWithValue("@Parcial", 0)

            ' Notas nulas
            cmd.Parameters.AddWithValue("@Notas1", DBNull.Value)
            cmd.Parameters.AddWithValue("@Notas2", DBNull.Value)
            cmd.Parameters.AddWithValue("@Notas3", DBNull.Value)
            cmd.Parameters.AddWithValue("@Notas4", DBNull.Value)
            cmd.Parameters.AddWithValue("@Notas5", DBNull.Value)
            cmd.Parameters.AddWithValue("@Notas6", DBNull.Value)
            cmd.Parameters.AddWithValue("@Notas7", DBNull.Value)
            cmd.Parameters.AddWithValue("@Notas8", DBNull.Value)
            cmd.Parameters.AddWithValue("@Notas9", DBNull.Value)
            cmd.Parameters.AddWithValue("@Notas10", DBNull.Value)
            cmd.Parameters.AddWithValue("@BaseImpuestD", DBNull.Value)

            cmd.ExecuteNonQuery()
        End Using
    End Sub



End Class