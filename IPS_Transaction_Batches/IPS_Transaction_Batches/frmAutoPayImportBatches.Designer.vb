<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAutoPayImportBatches
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
        Me.btnImport = New System.Windows.Forms.Button()
        Me.Header7 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Header6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Header5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Header4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Header3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Header2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Header1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.lvwDisplay = New System.Windows.Forms.ListView()
        Me.Header8 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.btnClose = New System.Windows.Forms.Button()
        Me.gbFilters = New System.Windows.Forms.GroupBox()
        Me.cmbBatchStatusFilter = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.rbNone = New System.Windows.Forms.RadioButton()
        Me.rb180Days = New System.Windows.Forms.RadioButton()
        Me.rb90Days = New System.Windows.Forms.RadioButton()
        Me.rb30Days = New System.Windows.Forms.RadioButton()
        Me.gbBatch = New System.Windows.Forms.GroupBox()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.cmbBatchStatus = New System.Windows.Forms.ComboBox()
        Me.lblDeclinedSum = New System.Windows.Forms.Label()
        Me.lblDeclinedCount = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.lblApprovedSum = New System.Windows.Forms.Label()
        Me.lblApprovedCount = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.lblInvalidCount = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.lblDuplicateSum = New System.Windows.Forms.Label()
        Me.lblDuplicateCount = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.lblAcceptedSum = New System.Windows.Forms.Label()
        Me.lblAcceptedCount = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.lblImportedSum = New System.Windows.Forms.Label()
        Me.lblImportedCount = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblImportDate = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btnProcessTrans = New System.Windows.Forms.Button()
        Me.gbFilters.SuspendLayout()
        Me.gbBatch.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnImport
        '
        Me.btnImport.Location = New System.Drawing.Point(726, 16)
        Me.btnImport.Name = "btnImport"
        Me.btnImport.Size = New System.Drawing.Size(104, 27)
        Me.btnImport.TabIndex = 18
        Me.btnImport.Text = "Import"
        Me.btnImport.UseVisualStyleBackColor = True
        '
        'Header7
        '
        Me.Header7.Text = "Invalid"
        Me.Header7.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Header7.Width = 65
        '
        'Header6
        '
        Me.Header6.Text = ""
        Me.Header6.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Header6.Width = 65
        '
        'Header5
        '
        Me.Header5.Text = "Duplicated"
        Me.Header5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Header5.Width = 65
        '
        'Header4
        '
        Me.Header4.Text = ""
        Me.Header4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Header4.Width = 70
        '
        'Header3
        '
        Me.Header3.Text = "Imported"
        Me.Header3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Header3.Width = 65
        '
        'Header2
        '
        Me.Header2.Text = "ImportedDate"
        Me.Header2.Width = 80
        '
        'Header1
        '
        Me.Header1.Text = "BatchID"
        '
        'lvwDisplay
        '
        Me.lvwDisplay.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.Header1, Me.Header8, Me.Header2, Me.Header3, Me.ColumnHeader1, Me.ColumnHeader2, Me.Header4, Me.Header5, Me.Header6, Me.Header7})
        Me.lvwDisplay.FullRowSelect = True
        Me.lvwDisplay.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lvwDisplay.HideSelection = False
        Me.lvwDisplay.Location = New System.Drawing.Point(9, 55)
        Me.lvwDisplay.MultiSelect = False
        Me.lvwDisplay.Name = "lvwDisplay"
        Me.lvwDisplay.ShowGroups = False
        Me.lvwDisplay.Size = New System.Drawing.Size(704, 192)
        Me.lvwDisplay.TabIndex = 17
        Me.lvwDisplay.UseCompatibleStateImageBehavior = False
        Me.lvwDisplay.View = System.Windows.Forms.View.Details
        '
        'Header8
        '
        Me.Header8.Text = "Status"
        Me.Header8.Width = 90
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = ""
        Me.ColumnHeader1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Accepted"
        Me.ColumnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(726, 45)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(104, 27)
        Me.btnClose.TabIndex = 20
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'gbFilters
        '
        Me.gbFilters.Controls.Add(Me.cmbBatchStatusFilter)
        Me.gbFilters.Controls.Add(Me.Label2)
        Me.gbFilters.Controls.Add(Me.Label1)
        Me.gbFilters.Controls.Add(Me.rbNone)
        Me.gbFilters.Controls.Add(Me.rb180Days)
        Me.gbFilters.Controls.Add(Me.rb90Days)
        Me.gbFilters.Controls.Add(Me.rb30Days)
        Me.gbFilters.Location = New System.Drawing.Point(9, 1)
        Me.gbFilters.Name = "gbFilters"
        Me.gbFilters.Size = New System.Drawing.Size(704, 49)
        Me.gbFilters.TabIndex = 25
        Me.gbFilters.TabStop = False
        Me.gbFilters.Text = "Filter:"
        '
        'cmbBatchStatusFilter
        '
        Me.cmbBatchStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBatchStatusFilter.FormattingEnabled = True
        Me.cmbBatchStatusFilter.Location = New System.Drawing.Point(96, 19)
        Me.cmbBatchStatusFilter.Name = "cmbBatchStatusFilter"
        Me.cmbBatchStatusFilter.Size = New System.Drawing.Size(196, 21)
        Me.cmbBatchStatusFilter.TabIndex = 9
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(19, 22)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(71, 13)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Batch Status:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(325, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(68, 13)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "Date Range:"
        '
        'rbNone
        '
        Me.rbNone.AutoSize = True
        Me.rbNone.Location = New System.Drawing.Point(633, 20)
        Me.rbNone.Name = "rbNone"
        Me.rbNone.Size = New System.Drawing.Size(51, 17)
        Me.rbNone.TabIndex = 3
        Me.rbNone.Text = "None"
        Me.rbNone.UseVisualStyleBackColor = True
        '
        'rb180Days
        '
        Me.rb180Days.AutoSize = True
        Me.rb180Days.Location = New System.Drawing.Point(552, 20)
        Me.rb180Days.Name = "rb180Days"
        Me.rb180Days.Size = New System.Drawing.Size(70, 17)
        Me.rb180Days.TabIndex = 2
        Me.rb180Days.Text = "180 Days"
        Me.rb180Days.UseVisualStyleBackColor = True
        '
        'rb90Days
        '
        Me.rb90Days.AutoSize = True
        Me.rb90Days.Location = New System.Drawing.Point(477, 20)
        Me.rb90Days.Name = "rb90Days"
        Me.rb90Days.Size = New System.Drawing.Size(64, 17)
        Me.rb90Days.TabIndex = 1
        Me.rb90Days.Text = "90 Days"
        Me.rb90Days.UseVisualStyleBackColor = True
        '
        'rb30Days
        '
        Me.rb30Days.AutoSize = True
        Me.rb30Days.Checked = True
        Me.rb30Days.Location = New System.Drawing.Point(402, 20)
        Me.rb30Days.Name = "rb30Days"
        Me.rb30Days.Size = New System.Drawing.Size(64, 17)
        Me.rb30Days.TabIndex = 0
        Me.rb30Days.TabStop = True
        Me.rb30Days.Text = "30 Days"
        Me.rb30Days.UseVisualStyleBackColor = True
        '
        'gbBatch
        '
        Me.gbBatch.Controls.Add(Me.btnCancel)
        Me.gbBatch.Controls.Add(Me.btnSave)
        Me.gbBatch.Controls.Add(Me.cmbBatchStatus)
        Me.gbBatch.Controls.Add(Me.lblDeclinedSum)
        Me.gbBatch.Controls.Add(Me.lblDeclinedCount)
        Me.gbBatch.Controls.Add(Me.Label15)
        Me.gbBatch.Controls.Add(Me.lblApprovedSum)
        Me.gbBatch.Controls.Add(Me.lblApprovedCount)
        Me.gbBatch.Controls.Add(Me.Label18)
        Me.gbBatch.Controls.Add(Me.Label20)
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
        Me.gbBatch.Controls.Add(Me.Label4)
        Me.gbBatch.Controls.Add(Me.btnProcessTrans)
        Me.gbBatch.Location = New System.Drawing.Point(9, 256)
        Me.gbBatch.Name = "gbBatch"
        Me.gbBatch.Size = New System.Drawing.Size(704, 134)
        Me.gbBatch.TabIndex = 26
        Me.gbBatch.TabStop = False
        Me.gbBatch.Text = "Batch ID:"
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(597, 91)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(92, 27)
        Me.btnCancel.TabIndex = 26
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(494, 91)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(92, 27)
        Me.btnSave.TabIndex = 25
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'cmbBatchStatus
        '
        Me.cmbBatchStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBatchStatus.FormattingEnabled = True
        Me.cmbBatchStatus.Location = New System.Drawing.Point(494, 18)
        Me.cmbBatchStatus.Name = "cmbBatchStatus"
        Me.cmbBatchStatus.Size = New System.Drawing.Size(195, 21)
        Me.cmbBatchStatus.TabIndex = 23
        '
        'lblDeclinedSum
        '
        Me.lblDeclinedSum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblDeclinedSum.Location = New System.Drawing.Point(591, 66)
        Me.lblDeclinedSum.Name = "lblDeclinedSum"
        Me.lblDeclinedSum.Size = New System.Drawing.Size(98, 15)
        Me.lblDeclinedSum.TabIndex = 22
        Me.lblDeclinedSum.Text = "$ 1,000,000.00"
        Me.lblDeclinedSum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblDeclinedCount
        '
        Me.lblDeclinedCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblDeclinedCount.Location = New System.Drawing.Point(494, 66)
        Me.lblDeclinedCount.Name = "lblDeclinedCount"
        Me.lblDeclinedCount.Size = New System.Drawing.Size(80, 15)
        Me.lblDeclinedCount.TabIndex = 21
        Me.lblDeclinedCount.Text = "1,000,000"
        Me.lblDeclinedCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(371, 66)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(95, 13)
        Me.Label15.TabIndex = 20
        Me.Label15.Text = "Declined Records:"
        '
        'lblApprovedSum
        '
        Me.lblApprovedSum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblApprovedSum.Location = New System.Drawing.Point(591, 46)
        Me.lblApprovedSum.Name = "lblApprovedSum"
        Me.lblApprovedSum.Size = New System.Drawing.Size(98, 15)
        Me.lblApprovedSum.TabIndex = 19
        Me.lblApprovedSum.Text = "$ 1,000,000.00"
        Me.lblApprovedSum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblApprovedCount
        '
        Me.lblApprovedCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblApprovedCount.Location = New System.Drawing.Point(494, 46)
        Me.lblApprovedCount.Name = "lblApprovedCount"
        Me.lblApprovedCount.Size = New System.Drawing.Size(80, 15)
        Me.lblApprovedCount.TabIndex = 18
        Me.lblApprovedCount.Text = "1,000,000"
        Me.lblApprovedCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(371, 46)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(99, 13)
        Me.Label18.TabIndex = 17
        Me.Label18.Text = "Approved Records:"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(371, 20)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(71, 13)
        Me.Label20.TabIndex = 15
        Me.Label20.Text = "Batch Status:"
        '
        'lblInvalidCount
        '
        Me.lblInvalidCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblInvalidCount.Location = New System.Drawing.Point(137, 107)
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
        'lblDuplicateSum
        '
        Me.lblDuplicateSum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblDuplicateSum.Location = New System.Drawing.Point(234, 87)
        Me.lblDuplicateSum.Name = "lblDuplicateSum"
        Me.lblDuplicateSum.Size = New System.Drawing.Size(98, 15)
        Me.lblDuplicateSum.TabIndex = 12
        Me.lblDuplicateSum.Text = "$ 1,000,000.00"
        Me.lblDuplicateSum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblDuplicateCount
        '
        Me.lblDuplicateCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblDuplicateCount.Location = New System.Drawing.Point(137, 87)
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
        Me.lblAcceptedSum.Location = New System.Drawing.Point(234, 67)
        Me.lblAcceptedSum.Name = "lblAcceptedSum"
        Me.lblAcceptedSum.Size = New System.Drawing.Size(98, 15)
        Me.lblAcceptedSum.TabIndex = 9
        Me.lblAcceptedSum.Text = "$ 1,000,000.00"
        Me.lblAcceptedSum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblAcceptedCount
        '
        Me.lblAcceptedCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblAcceptedCount.Location = New System.Drawing.Point(137, 67)
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
        Me.lblImportedSum.Location = New System.Drawing.Point(234, 47)
        Me.lblImportedSum.Name = "lblImportedSum"
        Me.lblImportedSum.Size = New System.Drawing.Size(98, 15)
        Me.lblImportedSum.TabIndex = 6
        Me.lblImportedSum.Text = "$ 1,000,000.00"
        Me.lblImportedSum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblImportedCount
        '
        Me.lblImportedCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblImportedCount.Location = New System.Drawing.Point(137, 47)
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
        Me.lblImportDate.Location = New System.Drawing.Point(137, 21)
        Me.lblImportDate.Name = "lblImportDate"
        Me.lblImportDate.Size = New System.Drawing.Size(80, 15)
        Me.lblImportDate.TabIndex = 3
        Me.lblImportDate.Text = "88/88/8888"
        Me.lblImportDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(14, 21)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(65, 13)
        Me.Label4.TabIndex = 2
        Me.Label4.Text = "Import Date:"
        '
        'btnProcessTrans
        '
        Me.btnProcessTrans.Location = New System.Drawing.Point(494, 91)
        Me.btnProcessTrans.Name = "btnProcessTrans"
        Me.btnProcessTrans.Size = New System.Drawing.Size(195, 27)
        Me.btnProcessTrans.TabIndex = 24
        Me.btnProcessTrans.Text = "Process Transactions"
        Me.btnProcessTrans.UseVisualStyleBackColor = True
        '
        'frmAutoPayImportBatches
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(841, 399)
        Me.ControlBox = False
        Me.Controls.Add(Me.gbBatch)
        Me.Controls.Add(Me.gbFilters)
        Me.Controls.Add(Me.btnImport)
        Me.Controls.Add(Me.lvwDisplay)
        Me.Controls.Add(Me.btnClose)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmAutoPayImportBatches"
        Me.Text = "AutoPay Import Batches"
        Me.gbFilters.ResumeLayout(False)
        Me.gbFilters.PerformLayout()
        Me.gbBatch.ResumeLayout(False)
        Me.gbBatch.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnImport As System.Windows.Forms.Button
    Friend WithEvents Header7 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Header6 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Header5 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Header4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Header3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Header2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Header1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents lvwDisplay As System.Windows.Forms.ListView
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents gbFilters As System.Windows.Forms.GroupBox
    Friend WithEvents rbNone As System.Windows.Forms.RadioButton
    Friend WithEvents rb180Days As System.Windows.Forms.RadioButton
    Friend WithEvents rb90Days As System.Windows.Forms.RadioButton
    Friend WithEvents rb30Days As System.Windows.Forms.RadioButton
    Friend WithEvents Header8 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents cmbBatchStatusFilter As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents gbBatch As System.Windows.Forms.GroupBox
    Friend WithEvents lblDeclinedSum As System.Windows.Forms.Label
    Friend WithEvents lblDeclinedCount As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents lblApprovedSum As System.Windows.Forms.Label
    Friend WithEvents lblApprovedCount As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents lblInvalidCount As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents lblDuplicateSum As System.Windows.Forms.Label
    Friend WithEvents lblDuplicateCount As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents lblAcceptedSum As System.Windows.Forms.Label
    Friend WithEvents lblAcceptedCount As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents lblImportedSum As System.Windows.Forms.Label
    Friend WithEvents lblImportedCount As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lblImportDate As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents cmbBatchStatus As System.Windows.Forms.ComboBox
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnProcessTrans As System.Windows.Forms.Button
End Class
