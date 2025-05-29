<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmProcesarPedido
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Label1 = New Label()
        cboClientes = New ComboBox()
        Label2 = New Label()
        txtCodCliente = New TextBox()
        Label3 = New Label()
        txtNombreCliente = New TextBox()
        Label4 = New Label()
        txtDireccion1 = New TextBox()
        Label5 = New Label()
        txtDireccion2 = New TextBox()
        Label6 = New Label()
        txtIdentificacion = New TextBox()
        btnSeleccionarExcel = New Button()
        dgvProductos = New DataGridView()
        Label7 = New Label()
        lblTotal = New Label()
        btnProcesar = New Button()
        TxtZona = New TextBox()
        Label8 = New Label()
        Label9 = New Label()
        TxtVend = New TextBox()
        CType(dgvProductos, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(14, 17)
        Label1.Margin = New Padding(4, 0, 4, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(47, 15)
        Label1.TabIndex = 0
        Label1.Text = "Cliente:"
        ' 
        ' cboClientes
        ' 
        cboClientes.DisplayMember = "Descrip"
        cboClientes.FormattingEnabled = True
        cboClientes.Location = New Point(70, 14)
        cboClientes.Margin = New Padding(4, 3, 4, 3)
        cboClientes.Name = "cboClientes"
        cboClientes.Size = New Size(349, 23)
        cboClientes.TabIndex = 1
        cboClientes.ValueMember = "CodClie"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(14, 48)
        Label2.Margin = New Padding(4, 0, 4, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(49, 15)
        Label2.TabIndex = 2
        Label2.Text = "Código:"
        ' 
        ' txtCodCliente
        ' 
        txtCodCliente.Location = New Point(70, 45)
        txtCodCliente.Margin = New Padding(4, 3, 4, 3)
        txtCodCliente.Name = "txtCodCliente"
        txtCodCliente.ReadOnly = True
        txtCodCliente.Size = New Size(116, 23)
        txtCodCliente.TabIndex = 3
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(194, 48)
        Label3.Margin = New Padding(4, 0, 4, 0)
        Label3.Name = "Label3"
        Label3.Size = New Size(54, 15)
        Label3.TabIndex = 4
        Label3.Text = "Nombre:"
        ' 
        ' txtNombreCliente
        ' 
        txtNombreCliente.Location = New Point(255, 45)
        txtNombreCliente.Margin = New Padding(4, 3, 4, 3)
        txtNombreCliente.Name = "txtNombreCliente"
        txtNombreCliente.ReadOnly = True
        txtNombreCliente.Size = New Size(349, 23)
        txtNombreCliente.TabIndex = 5
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(14, 78)
        Label4.Margin = New Padding(4, 0, 4, 0)
        Label4.Name = "Label4"
        Label4.Size = New Size(60, 15)
        Label4.TabIndex = 6
        Label4.Text = "Dirección:"
        ' 
        ' txtDireccion1
        ' 
        txtDireccion1.Location = New Point(85, 75)
        txtDireccion1.Margin = New Padding(4, 3, 4, 3)
        txtDireccion1.Name = "txtDireccion1"
        txtDireccion1.ReadOnly = True
        txtDireccion1.Size = New Size(349, 23)
        txtDireccion1.TabIndex = 7
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Location = New Point(442, 78)
        Label5.Margin = New Padding(4, 0, 4, 0)
        Label5.Name = "Label5"
        Label5.Size = New Size(69, 15)
        Label5.TabIndex = 8
        Label5.Text = "Dirección 2:"
        ' 
        ' txtDireccion2
        ' 
        txtDireccion2.Location = New Point(520, 75)
        txtDireccion2.Margin = New Padding(4, 3, 4, 3)
        txtDireccion2.Name = "txtDireccion2"
        txtDireccion2.ReadOnly = True
        txtDireccion2.Size = New Size(349, 23)
        txtDireccion2.TabIndex = 9
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Location = New Point(14, 108)
        Label6.Margin = New Padding(4, 0, 4, 0)
        Label6.Name = "Label6"
        Label6.Size = New Size(82, 15)
        Label6.TabIndex = 10
        Label6.Text = "Identificación:"
        ' 
        ' txtIdentificacion
        ' 
        txtIdentificacion.Location = New Point(106, 105)
        txtIdentificacion.Margin = New Padding(4, 3, 4, 3)
        txtIdentificacion.Name = "txtIdentificacion"
        txtIdentificacion.ReadOnly = True
        txtIdentificacion.Size = New Size(233, 23)
        txtIdentificacion.TabIndex = 11
        ' 
        ' btnSeleccionarExcel
        ' 
        btnSeleccionarExcel.ImageAlign = ContentAlignment.MiddleLeft
        btnSeleccionarExcel.Location = New Point(18, 135)
        btnSeleccionarExcel.Margin = New Padding(4, 3, 4, 3)
        btnSeleccionarExcel.Name = "btnSeleccionarExcel"
        btnSeleccionarExcel.Size = New Size(175, 35)
        btnSeleccionarExcel.TabIndex = 12
        btnSeleccionarExcel.Text = "Seleccionar Excel"
        btnSeleccionarExcel.UseVisualStyleBackColor = True
        ' 
        ' dgvProductos
        ' 
        dgvProductos.AllowUserToAddRows = False
        dgvProductos.AllowUserToDeleteRows = False
        dgvProductos.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        dgvProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvProductos.Location = New Point(18, 177)
        dgvProductos.Margin = New Padding(4, 3, 4, 3)
        dgvProductos.Name = "dgvProductos"
        dgvProductos.ReadOnly = True
        dgvProductos.Size = New Size(853, 288)
        dgvProductos.TabIndex = 13
        ' 
        ' Label7
        ' 
        Label7.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Label7.AutoSize = True
        Label7.Font = New Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label7.Location = New Point(677, 473)
        Label7.Margin = New Padding(4, 0, 4, 0)
        Label7.Name = "Label7"
        Label7.Size = New Size(40, 13)
        Label7.TabIndex = 14
        Label7.Text = "Total:"
        ' 
        ' lblTotal
        ' 
        lblTotal.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        lblTotal.Font = New Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lblTotal.Location = New Point(730, 473)
        lblTotal.Margin = New Padding(4, 0, 4, 0)
        lblTotal.Name = "lblTotal"
        lblTotal.Size = New Size(140, 15)
        lblTotal.TabIndex = 15
        lblTotal.Text = "$0.00"
        lblTotal.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' btnProcesar
        ' 
        btnProcesar.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnProcesar.ImageAlign = ContentAlignment.MiddleLeft
        btnProcesar.Location = New Point(695, 492)
        btnProcesar.Margin = New Padding(4, 3, 4, 3)
        btnProcesar.Name = "btnProcesar"
        btnProcesar.Size = New Size(175, 35)
        btnProcesar.TabIndex = 16
        btnProcesar.Text = "Procesar Pedido"
        btnProcesar.UseVisualStyleBackColor = True
        ' 
        ' TxtZona
        ' 
        TxtZona.Location = New Point(411, 105)
        TxtZona.Margin = New Padding(4, 3, 4, 3)
        TxtZona.Name = "TxtZona"
        TxtZona.ReadOnly = True
        TxtZona.Size = New Size(133, 23)
        TxtZona.TabIndex = 17
        ' 
        ' Label8
        ' 
        Label8.AutoSize = True
        Label8.Location = New Point(365, 108)
        Label8.Margin = New Padding(4, 0, 4, 0)
        Label8.Name = "Label8"
        Label8.Size = New Size(37, 15)
        Label8.TabIndex = 18
        Label8.Text = "Zona:"
        ' 
        ' Label9
        ' 
        Label9.AutoSize = True
        Label9.Location = New Point(590, 108)
        Label9.Margin = New Padding(4, 0, 4, 0)
        Label9.Name = "Label9"
        Label9.Size = New Size(60, 15)
        Label9.TabIndex = 20
        Label9.Text = "Vendedor:"
        ' 
        ' TxtVend
        ' 
        TxtVend.Location = New Point(658, 105)
        TxtVend.Margin = New Padding(4, 3, 4, 3)
        TxtVend.Name = "TxtVend"
        TxtVend.ReadOnly = True
        TxtVend.Size = New Size(211, 23)
        TxtVend.TabIndex = 19
        ' 
        ' frmProcesarPedido
        ' 
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(884, 540)
        Controls.Add(Label9)
        Controls.Add(TxtVend)
        Controls.Add(Label8)
        Controls.Add(TxtZona)
        Controls.Add(btnProcesar)
        Controls.Add(lblTotal)
        Controls.Add(Label7)
        Controls.Add(dgvProductos)
        Controls.Add(btnSeleccionarExcel)
        Controls.Add(txtIdentificacion)
        Controls.Add(Label6)
        Controls.Add(txtDireccion2)
        Controls.Add(Label5)
        Controls.Add(txtDireccion1)
        Controls.Add(Label4)
        Controls.Add(txtNombreCliente)
        Controls.Add(Label3)
        Controls.Add(txtCodCliente)
        Controls.Add(Label2)
        Controls.Add(cboClientes)
        Controls.Add(Label1)
        Margin = New Padding(4, 3, 4, 3)
        Name = "frmProcesarPedido"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Procesador de Pedidos"
        CType(dgvProductos, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cboClientes As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtCodCliente As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtNombreCliente As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtDireccion1 As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtDireccion2 As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtIdentificacion As System.Windows.Forms.TextBox
    Friend WithEvents btnSeleccionarExcel As System.Windows.Forms.Button
    Friend WithEvents dgvProductos As System.Windows.Forms.DataGridView
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents lblTotal As System.Windows.Forms.Label
    Friend WithEvents btnProcesar As System.Windows.Forms.Button
    Friend WithEvents TxtZona As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents TxtVend As TextBox
End Class