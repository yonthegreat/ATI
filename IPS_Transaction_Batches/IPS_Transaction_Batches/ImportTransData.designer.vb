<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TransDataImport
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
        Me.btnBrowse = New System.Windows.Forms.Button
        Me.gbFilename = New System.Windows.Forms.GroupBox
        Me.txtFileName = New System.Windows.Forms.TextBox
        Me.gbBatch = New System.Windows.Forms.GroupBox
        Me.Label16 = New System.Windows.Forms.Label
        Me.lblDuplicates = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.lblRecords = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.lblImportDate = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnImport = New System.Windows.Forms.Button
        Me.btnClose = New System.Windows.Forms.Button
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.lblAcceptedAmounts = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.lblAccepted = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.GroupBox4 = New System.Windows.Forms.GroupBox
        Me.lblRejectedAmounts = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.lblRejected = New System.Windows.Forms.Label
        Me.Label11 = New System.Windows.Forms.Label
        Me.GroupBox5 = New System.Windows.Forms.GroupBox
        Me.lblVoided = New System.Windows.Forms.Label
        Me.Label19 = New System.Windows.Forms.Label
        Me.GroupBox6 = New System.Windows.Forms.GroupBox
        Me.lblCreditedAmounts = New System.Windows.Forms.Label
        Me.Label13 = New System.Windows.Forms.Label
        Me.lblCredited = New System.Windows.Forms.Label
        Me.Label15 = New System.Windows.Forms.Label
        Me.gbBatch.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnBrowse
        '
        Me.btnBrowse.Location = New System.Drawing.Point(395, 26)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowse.TabIndex = 0
        Me.btnBrowse.Text = "Browse..."
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'gbFilename
        '
        Me.gbFilename.Location = New System.Drawing.Point(12, 9)
        Me.gbFilename.Name = "gbFilename"
        Me.gbFilename.Size = New System.Drawing.Size(469, 48)
        Me.gbFilename.TabIndex = 3
        Me.gbFilename.TabStop = False
        Me.gbFilename.Text = "Transaction Data File:"
        '
        'txtFileName
        '
        Me.txtFileName.Location = New System.Drawing.Point(24, 28)
        Me.txtFileName.Name = "txtFileName"
        Me.txtFileName.Size = New System.Drawing.Size(363, 20)
        Me.txtFileName.TabIndex = 4
        Me.txtFileName.Text = "C:\SearchResults.txt"
        '
        'gbBatch
        '
        Me.gbBatch.Controls.Add(Me.Label16)
        Me.gbBatch.Controls.Add(Me.lblDuplicates)
        Me.gbBatch.Controls.Add(Me.Label4)
        Me.gbBatch.Controls.Add(Me.lblRecords)
        Me.gbBatch.Controls.Add(Me.Label3)
        Me.gbBatch.Controls.Add(Me.lblImportDate)
        Me.gbBatch.Controls.Add(Me.Label1)
        Me.gbBatch.Location = New System.Drawing.Point(12, 58)
        Me.gbBatch.Name = "gbBatch"
        Me.gbBatch.Size = New System.Drawing.Size(469, 43)
        Me.gbBatch.TabIndex = 5
        Me.gbBatch.TabStop = False
        Me.gbBatch.Text = "Batch ID:"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(408, 21)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(48, 13)
        Me.Label16.TabIndex = 8
        Me.Label16.Text = "(ignored)"
        '
        'lblDuplicates
        '
        Me.lblDuplicates.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblDuplicates.Location = New System.Drawing.Point(353, 21)
        Me.lblDuplicates.Name = "lblDuplicates"
        Me.lblDuplicates.Size = New System.Drawing.Size(53, 15)
        Me.lblDuplicates.TabIndex = 7
        Me.lblDuplicates.Text = "0"
        Me.lblDuplicates.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(285, 21)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(60, 13)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Duplicates:"
        '
        'lblRecords
        '
        Me.lblRecords.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblRecords.Location = New System.Drawing.Point(222, 21)
        Me.lblRecords.Name = "lblRecords"
        Me.lblRecords.Size = New System.Drawing.Size(53, 15)
        Me.lblRecords.TabIndex = 5
        Me.lblRecords.Text = "0"
        Me.lblRecords.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(158, 21)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(50, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Records:"
        '
        'lblImportDate
        '
        Me.lblImportDate.AutoSize = True
        Me.lblImportDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblImportDate.Location = New System.Drawing.Point(73, 21)
        Me.lblImportDate.Name = "lblImportDate"
        Me.lblImportDate.Size = New System.Drawing.Size(67, 15)
        Me.lblImportDate.TabIndex = 3
        Me.lblImportDate.Text = "88/88/8888"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(14, 21)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(51, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Imported:"
        '
        'btnImport
        '
        Me.btnImport.Location = New System.Drawing.Point(489, 12)
        Me.btnImport.Name = "btnImport"
        Me.btnImport.Size = New System.Drawing.Size(104, 27)
        Me.btnImport.TabIndex = 6
        Me.btnImport.Text = "Import"
        Me.btnImport.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(489, 40)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(104, 27)
        Me.btnClose.TabIndex = 7
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.lblAcceptedAmounts)
        Me.GroupBox3.Controls.Add(Me.Label7)
        Me.GroupBox3.Controls.Add(Me.lblAccepted)
        Me.GroupBox3.Controls.Add(Me.Label5)
        Me.GroupBox3.Location = New System.Drawing.Point(13, 107)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(140, 81)
        Me.GroupBox3.TabIndex = 8
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Accepted Transactions:"
        '
        'lblAcceptedAmounts
        '
        Me.lblAcceptedAmounts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblAcceptedAmounts.Location = New System.Drawing.Point(60, 50)
        Me.lblAcceptedAmounts.Name = "lblAcceptedAmounts"
        Me.lblAcceptedAmounts.Size = New System.Drawing.Size(68, 15)
        Me.lblAcceptedAmounts.TabIndex = 9
        Me.lblAcceptedAmounts.Text = "0.00"
        Me.lblAcceptedAmounts.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(12, 50)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(46, 13)
        Me.Label7.TabIndex = 8
        Me.Label7.Text = "Amount:"
        '
        'lblAccepted
        '
        Me.lblAccepted.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblAccepted.Location = New System.Drawing.Point(75, 26)
        Me.lblAccepted.Name = "lblAccepted"
        Me.lblAccepted.Size = New System.Drawing.Size(53, 15)
        Me.lblAccepted.TabIndex = 7
        Me.lblAccepted.Text = "0"
        Me.lblAccepted.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(12, 26)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(38, 13)
        Me.Label5.TabIndex = 6
        Me.Label5.Text = "Count:"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.lblRejectedAmounts)
        Me.GroupBox4.Controls.Add(Me.Label9)
        Me.GroupBox4.Controls.Add(Me.lblRejected)
        Me.GroupBox4.Controls.Add(Me.Label11)
        Me.GroupBox4.Location = New System.Drawing.Point(159, 107)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(140, 81)
        Me.GroupBox4.TabIndex = 9
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Rejected Transactions:"
        '
        'lblRejectedAmounts
        '
        Me.lblRejectedAmounts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblRejectedAmounts.Location = New System.Drawing.Point(60, 50)
        Me.lblRejectedAmounts.Name = "lblRejectedAmounts"
        Me.lblRejectedAmounts.Size = New System.Drawing.Size(68, 15)
        Me.lblRejectedAmounts.TabIndex = 13
        Me.lblRejectedAmounts.Text = "0.00"
        Me.lblRejectedAmounts.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(12, 50)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(46, 13)
        Me.Label9.TabIndex = 12
        Me.Label9.Text = "Amount:"
        '
        'lblRejected
        '
        Me.lblRejected.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblRejected.Location = New System.Drawing.Point(75, 26)
        Me.lblRejected.Name = "lblRejected"
        Me.lblRejected.Size = New System.Drawing.Size(53, 15)
        Me.lblRejected.TabIndex = 11
        Me.lblRejected.Text = "0"
        Me.lblRejected.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(12, 26)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(38, 13)
        Me.Label11.TabIndex = 10
        Me.Label11.Text = "Count:"
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.lblVoided)
        Me.GroupBox5.Controls.Add(Me.Label19)
        Me.GroupBox5.Location = New System.Drawing.Point(451, 107)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(140, 81)
        Me.GroupBox5.TabIndex = 10
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Voided Transactions:"
        '
        'lblVoided
        '
        Me.lblVoided.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblVoided.Location = New System.Drawing.Point(75, 26)
        Me.lblVoided.Name = "lblVoided"
        Me.lblVoided.Size = New System.Drawing.Size(53, 15)
        Me.lblVoided.TabIndex = 11
        Me.lblVoided.Text = "0"
        Me.lblVoided.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(12, 26)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(38, 13)
        Me.Label19.TabIndex = 10
        Me.Label19.Text = "Count:"
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.lblCreditedAmounts)
        Me.GroupBox6.Controls.Add(Me.Label13)
        Me.GroupBox6.Controls.Add(Me.lblCredited)
        Me.GroupBox6.Controls.Add(Me.Label15)
        Me.GroupBox6.Location = New System.Drawing.Point(305, 107)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(140, 81)
        Me.GroupBox6.TabIndex = 11
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Credited Transactions:"
        '
        'lblCreditedAmounts
        '
        Me.lblCreditedAmounts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblCreditedAmounts.Location = New System.Drawing.Point(60, 50)
        Me.lblCreditedAmounts.Name = "lblCreditedAmounts"
        Me.lblCreditedAmounts.Size = New System.Drawing.Size(68, 15)
        Me.lblCreditedAmounts.TabIndex = 13
        Me.lblCreditedAmounts.Text = "0.00"
        Me.lblCreditedAmounts.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(12, 50)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(46, 13)
        Me.Label13.TabIndex = 12
        Me.Label13.Text = "Amount:"
        '
        'lblCredited
        '
        Me.lblCredited.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblCredited.Location = New System.Drawing.Point(75, 26)
        Me.lblCredited.Name = "lblCredited"
        Me.lblCredited.Size = New System.Drawing.Size(53, 15)
        Me.lblCredited.TabIndex = 11
        Me.lblCredited.Text = "0"
        Me.lblCredited.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(12, 26)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(38, 13)
        Me.Label15.TabIndex = 10
        Me.Label15.Text = "Count:"
        '
        'TransDataImport
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(603, 198)
        Me.Controls.Add(Me.GroupBox6)
        Me.Controls.Add(Me.GroupBox5)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.txtFileName)
        Me.Controls.Add(Me.btnBrowse)
        Me.Controls.Add(Me.gbFilename)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnImport)
        Me.Controls.Add(Me.gbBatch)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "TransDataImport"
        Me.Text = "Import Merchant Transactions"
        Me.gbBatch.ResumeLayout(False)
        Me.gbBatch.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents gbFilename As System.Windows.Forms.GroupBox
    Friend WithEvents txtFileName As System.Windows.Forms.TextBox
    Friend WithEvents gbBatch As System.Windows.Forms.GroupBox
    Friend WithEvents lblImportDate As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnImport As System.Windows.Forms.Button
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents lblRecords As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lblDuplicates As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
    Friend WithEvents lblAcceptedAmounts As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents lblAccepted As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents lblRejectedAmounts As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents lblRejected As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents lblVoided As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents lblCreditedAmounts As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents lblCredited As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
End Class
