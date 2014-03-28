<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmImportAutoPayAccountData
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnImport = New System.Windows.Forms.Button()
        Me.lblImportedCount = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblImportDate = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtFileName = New System.Windows.Forms.TextBox()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.gbFilename = New System.Windows.Forms.GroupBox()
        Me.gbBatch = New System.Windows.Forms.GroupBox()
        Me.lblDuplicateSum = New System.Windows.Forms.Label()
        Me.lblDuplicateCount = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.lblAcceptedSum = New System.Windows.Forms.Label()
        Me.lblAcceptedCount = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.lblImportedSum = New System.Windows.Forms.Label()
        Me.lblInvalidCount = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.gbBatch.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(457, 38)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(104, 27)
        Me.btnClose.TabIndex = 17
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnImport
        '
        Me.btnImport.Location = New System.Drawing.Point(457, 10)
        Me.btnImport.Name = "btnImport"
        Me.btnImport.Size = New System.Drawing.Size(104, 27)
        Me.btnImport.TabIndex = 16
        Me.btnImport.Text = "Import"
        Me.btnImport.UseVisualStyleBackColor = True
        '
        'lblImportedCount
        '
        Me.lblImportedCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblImportedCount.Location = New System.Drawing.Point(149, 47)
        Me.lblImportedCount.Name = "lblImportedCount"
        Me.lblImportedCount.Size = New System.Drawing.Size(80, 15)
        Me.lblImportedCount.TabIndex = 5
        Me.lblImportedCount.Text = "1,000,000"
        Me.lblImportedCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(14, 47)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(94, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Imported Records:"
        '
        'lblImportDate
        '
        Me.lblImportDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblImportDate.Location = New System.Drawing.Point(149, 21)
        Me.lblImportDate.Name = "lblImportDate"
        Me.lblImportDate.Size = New System.Drawing.Size(80, 15)
        Me.lblImportDate.TabIndex = 3
        Me.lblImportDate.Text = "88/88/8888"
        Me.lblImportDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(14, 21)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(65, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Import Date:"
        '
        'txtFileName
        '
        Me.txtFileName.Location = New System.Drawing.Point(24, 26)
        Me.txtFileName.Name = "txtFileName"
        Me.txtFileName.Size = New System.Drawing.Size(329, 20)
        Me.txtFileName.TabIndex = 14
        Me.txtFileName.Text = "C:\CCAPAY20130917.txt"
        '
        'btnBrowse
        '
        Me.btnBrowse.Location = New System.Drawing.Point(359, 24)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowse.TabIndex = 12
        Me.btnBrowse.Text = "Browse..."
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'gbFilename
        '
        Me.gbFilename.Location = New System.Drawing.Point(12, 7)
        Me.gbFilename.Name = "gbFilename"
        Me.gbFilename.Size = New System.Drawing.Size(431, 48)
        Me.gbFilename.TabIndex = 13
        Me.gbFilename.TabStop = False
        Me.gbFilename.Text = "Pending Transaction Data File:"
        '
        'gbBatch
        '
        Me.gbBatch.Controls.Add(Me.lblInvalidCount)
        Me.gbBatch.Controls.Add(Me.Label7)
        Me.gbBatch.Controls.Add(Me.lblDuplicateSum)
        Me.gbBatch.Controls.Add(Me.lblDuplicateCount)
        Me.gbBatch.Controls.Add(Me.Label6)
        Me.gbBatch.Controls.Add(Me.lblAcceptedSum)
        Me.gbBatch.Controls.Add(Me.lblAcceptedCount)
        Me.gbBatch.Controls.Add(Me.Label5)
        Me.gbBatch.Controls.Add(Me.lblImportedSum)
        Me.gbBatch.Controls.Add(Me.lblImportedCount)
        Me.gbBatch.Controls.Add(Me.Label3)
        Me.gbBatch.Controls.Add(Me.lblImportDate)
        Me.gbBatch.Controls.Add(Me.Label1)
        Me.gbBatch.Location = New System.Drawing.Point(12, 56)
        Me.gbBatch.Name = "gbBatch"
        Me.gbBatch.Size = New System.Drawing.Size(431, 130)
        Me.gbBatch.TabIndex = 15
        Me.gbBatch.TabStop = False
        Me.gbBatch.Text = "Batch ID:"
        '
        'lblDuplicateSum
        '
        Me.lblDuplicateSum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblDuplicateSum.Location = New System.Drawing.Point(277, 87)
        Me.lblDuplicateSum.Name = "lblDuplicateSum"
        Me.lblDuplicateSum.Size = New System.Drawing.Size(98, 15)
        Me.lblDuplicateSum.TabIndex = 12
        Me.lblDuplicateSum.Text = "$ 1,000,000.00"
        Me.lblDuplicateSum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblDuplicateCount
        '
        Me.lblDuplicateCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblDuplicateCount.Location = New System.Drawing.Point(149, 87)
        Me.lblDuplicateCount.Name = "lblDuplicateCount"
        Me.lblDuplicateCount.Size = New System.Drawing.Size(80, 15)
        Me.lblDuplicateCount.TabIndex = 11
        Me.lblDuplicateCount.Text = "1,000,000"
        Me.lblDuplicateCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(14, 87)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(98, 13)
        Me.Label6.TabIndex = 10
        Me.Label6.Text = "Duplicate Records:"
        '
        'lblAcceptedSum
        '
        Me.lblAcceptedSum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblAcceptedSum.Location = New System.Drawing.Point(277, 67)
        Me.lblAcceptedSum.Name = "lblAcceptedSum"
        Me.lblAcceptedSum.Size = New System.Drawing.Size(98, 15)
        Me.lblAcceptedSum.TabIndex = 9
        Me.lblAcceptedSum.Text = "$ 1,000,000.00"
        Me.lblAcceptedSum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblAcceptedCount
        '
        Me.lblAcceptedCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblAcceptedCount.Location = New System.Drawing.Point(149, 67)
        Me.lblAcceptedCount.Name = "lblAcceptedCount"
        Me.lblAcceptedCount.Size = New System.Drawing.Size(80, 15)
        Me.lblAcceptedCount.TabIndex = 8
        Me.lblAcceptedCount.Text = "1,000,000"
        Me.lblAcceptedCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(14, 67)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(99, 13)
        Me.Label5.TabIndex = 7
        Me.Label5.Text = "Accepted Records:"
        '
        'lblImportedSum
        '
        Me.lblImportedSum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblImportedSum.Location = New System.Drawing.Point(277, 47)
        Me.lblImportedSum.Name = "lblImportedSum"
        Me.lblImportedSum.Size = New System.Drawing.Size(98, 15)
        Me.lblImportedSum.TabIndex = 6
        Me.lblImportedSum.Text = "$ 1,000,000.00"
        Me.lblImportedSum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblInvalidCount
        '
        Me.lblInvalidCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblInvalidCount.Location = New System.Drawing.Point(149, 107)
        Me.lblInvalidCount.Name = "lblInvalidCount"
        Me.lblInvalidCount.Size = New System.Drawing.Size(80, 15)
        Me.lblInvalidCount.TabIndex = 14
        Me.lblInvalidCount.Text = "1,000,000"
        Me.lblInvalidCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(14, 107)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(84, 13)
        Me.Label7.TabIndex = 13
        Me.Label7.Text = "Invalid Records:"
        '
        'frmImportAutoPayAccountData
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(574, 199)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnImport)
        Me.Controls.Add(Me.txtFileName)
        Me.Controls.Add(Me.btnBrowse)
        Me.Controls.Add(Me.gbFilename)
        Me.Controls.Add(Me.gbBatch)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmImportAutoPayAccountData"
        Me.Text = "Import AutoPay Account Data"
        Me.gbBatch.ResumeLayout(False)
        Me.gbBatch.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents btnImport As System.Windows.Forms.Button
    Friend WithEvents lblImportedCount As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lblImportDate As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtFileName As System.Windows.Forms.TextBox
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents gbFilename As System.Windows.Forms.GroupBox
    Friend WithEvents gbBatch As System.Windows.Forms.GroupBox
    Friend WithEvents lblImportedSum As System.Windows.Forms.Label
    Friend WithEvents lblDuplicateSum As System.Windows.Forms.Label
    Friend WithEvents lblDuplicateCount As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents lblAcceptedSum As System.Windows.Forms.Label
    Friend WithEvents lblAcceptedCount As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents lblInvalidCount As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
End Class
