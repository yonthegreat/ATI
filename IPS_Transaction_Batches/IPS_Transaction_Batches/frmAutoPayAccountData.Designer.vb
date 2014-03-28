<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAutoPayAccountData
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
        Me.btnSave = New System.Windows.Forms.Button()
        Me.cmbRecordStatus = New System.Windows.Forms.ComboBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.lblAutoPayDate = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btnProcessTrans = New System.Windows.Forms.Button()
        Me.Header3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Header2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Header1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.lvwDisplay = New System.Windows.Forms.ListView()
        Me.Header8 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.btnClose = New System.Windows.Forms.Button()
        Me.gbBatch = New System.Windows.Forms.GroupBox()
        Me.lblAutoPayAccountID = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.lblAutoPayAmount = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.gbFilters = New System.Windows.Forms.GroupBox()
        Me.cmbBatchDisplayed = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cmbBatchStatusFilter = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.rbNone = New System.Windows.Forms.RadioButton()
        Me.rb180Days = New System.Windows.Forms.RadioButton()
        Me.rb90Days = New System.Windows.Forms.RadioButton()
        Me.rb30Days = New System.Windows.Forms.RadioButton()
        Me.gbBatch.SuspendLayout()
        Me.gbFilters.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(137, 112)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(92, 27)
        Me.btnSave.TabIndex = 25
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'cmbRecordStatus
        '
        Me.cmbRecordStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbRecordStatus.FormattingEnabled = True
        Me.cmbRecordStatus.Location = New System.Drawing.Point(137, 86)
        Me.cmbRecordStatus.Name = "cmbRecordStatus"
        Me.cmbRecordStatus.Size = New System.Drawing.Size(195, 21)
        Me.cmbRecordStatus.TabIndex = 23
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(14, 88)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(78, 13)
        Me.Label20.TabIndex = 15
        Me.Label20.Text = "Record Status:"
        '
        'lblAutoPayDate
        '
        Me.lblAutoPayDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblAutoPayDate.Location = New System.Drawing.Point(137, 21)
        Me.lblAutoPayDate.Name = "lblAutoPayDate"
        Me.lblAutoPayDate.Size = New System.Drawing.Size(80, 15)
        Me.lblAutoPayDate.TabIndex = 3
        Me.lblAutoPayDate.Text = "88/88/8888"
        Me.lblAutoPayDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(14, 21)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(76, 13)
        Me.Label4.TabIndex = 2
        Me.Label4.Text = "AutoPay Date:"
        '
        'btnProcessTrans
        '
        Me.btnProcessTrans.Location = New System.Drawing.Point(137, 112)
        Me.btnProcessTrans.Name = "btnProcessTrans"
        Me.btnProcessTrans.Size = New System.Drawing.Size(195, 27)
        Me.btnProcessTrans.TabIndex = 24
        Me.btnProcessTrans.Text = "Process Transaction"
        Me.btnProcessTrans.UseVisualStyleBackColor = True
        '
        'Header3
        '
        Me.Header3.Text = "First Name"
        Me.Header3.Width = 132
        '
        'Header2
        '
        Me.Header2.Text = "AutoPay Date"
        Me.Header2.Width = 85
        '
        'Header1
        '
        Me.Header1.Text = "RecordID"
        Me.Header1.Width = 70
        '
        'lvwDisplay
        '
        Me.lvwDisplay.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.Header1, Me.Header8, Me.Header2, Me.ColumnHeader3, Me.Header3, Me.ColumnHeader1, Me.ColumnHeader2})
        Me.lvwDisplay.FullRowSelect = True
        Me.lvwDisplay.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lvwDisplay.HideSelection = False
        Me.lvwDisplay.Location = New System.Drawing.Point(9, 75)
        Me.lvwDisplay.MultiSelect = False
        Me.lvwDisplay.Name = "lvwDisplay"
        Me.lvwDisplay.ShowGroups = False
        Me.lvwDisplay.Size = New System.Drawing.Size(704, 192)
        Me.lvwDisplay.TabIndex = 27
        Me.lvwDisplay.UseCompatibleStateImageBehavior = False
        Me.lvwDisplay.View = System.Windows.Forms.View.Details
        '
        'Header8
        '
        Me.Header8.Text = "Status"
        Me.Header8.Width = 100
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "AccountID"
        Me.ColumnHeader3.Width = 80
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Last Name"
        Me.ColumnHeader1.Width = 132
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Amount"
        Me.ColumnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.ColumnHeader2.Width = 75
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(726, 15)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(104, 27)
        Me.btnClose.TabIndex = 29
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'gbBatch
        '
        Me.gbBatch.Controls.Add(Me.lblAutoPayAccountID)
        Me.gbBatch.Controls.Add(Me.Label7)
        Me.gbBatch.Controls.Add(Me.lblAutoPayAmount)
        Me.gbBatch.Controls.Add(Me.Label6)
        Me.gbBatch.Controls.Add(Me.btnCancel)
        Me.gbBatch.Controls.Add(Me.btnSave)
        Me.gbBatch.Controls.Add(Me.cmbRecordStatus)
        Me.gbBatch.Controls.Add(Me.Label20)
        Me.gbBatch.Controls.Add(Me.lblAutoPayDate)
        Me.gbBatch.Controls.Add(Me.Label4)
        Me.gbBatch.Controls.Add(Me.btnProcessTrans)
        Me.gbBatch.Location = New System.Drawing.Point(9, 276)
        Me.gbBatch.Name = "gbBatch"
        Me.gbBatch.Size = New System.Drawing.Size(360, 144)
        Me.gbBatch.TabIndex = 31
        Me.gbBatch.TabStop = False
        Me.gbBatch.Text = "Record ID:"
        '
        'lblAutoPayAccountID
        '
        Me.lblAutoPayAccountID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblAutoPayAccountID.Location = New System.Drawing.Point(137, 65)
        Me.lblAutoPayAccountID.Name = "lblAutoPayAccountID"
        Me.lblAutoPayAccountID.Size = New System.Drawing.Size(80, 15)
        Me.lblAutoPayAccountID.TabIndex = 30
        Me.lblAutoPayAccountID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(14, 65)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(61, 13)
        Me.Label7.TabIndex = 29
        Me.Label7.Text = "AccountID:"
        '
        'lblAutoPayAmount
        '
        Me.lblAutoPayAmount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblAutoPayAmount.Location = New System.Drawing.Point(137, 43)
        Me.lblAutoPayAmount.Name = "lblAutoPayAmount"
        Me.lblAutoPayAmount.Size = New System.Drawing.Size(80, 15)
        Me.lblAutoPayAmount.TabIndex = 28
        Me.lblAutoPayAmount.Text = "$ 1,000,000"
        Me.lblAutoPayAmount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(14, 43)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(89, 13)
        Me.Label6.TabIndex = 27
        Me.Label6.Text = "AutoPay Amount:"
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(240, 112)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(92, 27)
        Me.btnCancel.TabIndex = 26
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'gbFilters
        '
        Me.gbFilters.Controls.Add(Me.cmbBatchDisplayed)
        Me.gbFilters.Controls.Add(Me.Label3)
        Me.gbFilters.Controls.Add(Me.cmbBatchStatusFilter)
        Me.gbFilters.Controls.Add(Me.Label2)
        Me.gbFilters.Controls.Add(Me.Label1)
        Me.gbFilters.Controls.Add(Me.rbNone)
        Me.gbFilters.Controls.Add(Me.rb180Days)
        Me.gbFilters.Controls.Add(Me.rb90Days)
        Me.gbFilters.Controls.Add(Me.rb30Days)
        Me.gbFilters.Location = New System.Drawing.Point(9, 2)
        Me.gbFilters.Name = "gbFilters"
        Me.gbFilters.Size = New System.Drawing.Size(704, 71)
        Me.gbFilters.TabIndex = 32
        Me.gbFilters.TabStop = False
        Me.gbFilters.Text = "Filter:"
        '
        'cmbBatchDisplayed
        '
        Me.cmbBatchDisplayed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBatchDisplayed.FormattingEnabled = True
        Me.cmbBatchDisplayed.Location = New System.Drawing.Point(108, 43)
        Me.cmbBatchDisplayed.Name = "cmbBatchDisplayed"
        Me.cmbBatchDisplayed.Size = New System.Drawing.Size(514, 21)
        Me.cmbBatchDisplayed.TabIndex = 11
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(19, 46)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(87, 13)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "Batch Displayed:"
        '
        'cmbBatchStatusFilter
        '
        Me.cmbBatchStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBatchStatusFilter.FormattingEnabled = True
        Me.cmbBatchStatusFilter.Location = New System.Drawing.Point(108, 19)
        Me.cmbBatchStatusFilter.Name = "cmbBatchStatusFilter"
        Me.cmbBatchStatusFilter.Size = New System.Drawing.Size(186, 21)
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
        'frmAutoPayAccountData
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(851, 431)
        Me.ControlBox = False
        Me.Controls.Add(Me.gbFilters)
        Me.Controls.Add(Me.lvwDisplay)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.gbBatch)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmAutoPayAccountData"
        Me.Text = "AutoPay Account Data"
        Me.gbBatch.ResumeLayout(False)
        Me.gbBatch.PerformLayout()
        Me.gbFilters.ResumeLayout(False)
        Me.gbFilters.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents cmbRecordStatus As System.Windows.Forms.ComboBox
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents lblAutoPayDate As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents btnProcessTrans As System.Windows.Forms.Button
    Friend WithEvents Header3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Header2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Header1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents lvwDisplay As System.Windows.Forms.ListView
    Friend WithEvents Header8 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents gbBatch As System.Windows.Forms.GroupBox
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents gbFilters As System.Windows.Forms.GroupBox
    Friend WithEvents cmbBatchDisplayed As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents cmbBatchStatusFilter As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents rbNone As System.Windows.Forms.RadioButton
    Friend WithEvents rb180Days As System.Windows.Forms.RadioButton
    Friend WithEvents rb90Days As System.Windows.Forms.RadioButton
    Friend WithEvents rb30Days As System.Windows.Forms.RadioButton
    Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents lblAutoPayAmount As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents lblAutoPayAccountID As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label

End Class
