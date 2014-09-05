/////////////////////////////////////////////////////////////////////////////////
// PCX Plugin for Paint.NET
// Copyright (C) 2006 Joshua Bell (inexorabletash@gmail.com)
// Portions Copyright (C) 2006 Rick Brewster, et. al.
// See License.txt for complete licensing and attribution information.
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PaintDotNet;

namespace PcxFileTypePlugin
{
	public class PcxSaveConfigWidget
		: PaintDotNet.SaveConfigWidget
	{
		private System.Windows.Forms.TrackBar thresholdSlider;
		private System.Windows.Forms.Label thresholdLabel;
		private System.Windows.Forms.NumericUpDown thresholdUpDown;
		private System.Windows.Forms.CheckBox preMultiplyAlphaCheckBox;
		private System.Windows.Forms.CheckBox useOriginalPaletteCheckBox;
		private System.Windows.Forms.NumericUpDown ditherUpDown;
		private System.Windows.Forms.Label ditherLabel;
		private System.Windows.Forms.TrackBar ditherSlider;
		private System.ComponentModel.IContainer components = null;

		public PcxSaveConfigWidget()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		protected override void InitFileType()
		{
			this.fileType = new PcxFileType();
		}

		protected override void InitTokenFromWidget()
		{
			( (PcxSaveConfigToken)this.Token ).Threshold = this.thresholdSlider.Value;
			( (PcxSaveConfigToken)this.Token ).DitherLevel = this.ditherSlider.Value;
			( (PcxSaveConfigToken)this.Token ).PreMultiplyAlpha = this.preMultiplyAlphaCheckBox.Checked;
			( (PcxSaveConfigToken)this.Token ).UseOriginalPalette = this.useOriginalPaletteCheckBox.Checked;
		}

		protected override void InitWidgetFromToken( PaintDotNet.SaveConfigToken token )
		{
			this.thresholdSlider.Value = ( (PcxSaveConfigToken)token ).Threshold;
			this.ditherSlider.Value = ( (PcxSaveConfigToken)token ).DitherLevel;
			this.preMultiplyAlphaCheckBox.Checked = ( (PcxSaveConfigToken)token ).PreMultiplyAlpha;
			this.useOriginalPaletteCheckBox.Checked = ( (PcxSaveConfigToken)token ).UseOriginalPalette;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
				{
					components.Dispose();
					components = null;
				}
			}

			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.thresholdSlider = new System.Windows.Forms.TrackBar();
			this.thresholdLabel = new System.Windows.Forms.Label();
			this.thresholdUpDown = new System.Windows.Forms.NumericUpDown();
			this.preMultiplyAlphaCheckBox = new System.Windows.Forms.CheckBox();
			this.useOriginalPaletteCheckBox = new System.Windows.Forms.CheckBox();
			this.ditherUpDown = new System.Windows.Forms.NumericUpDown();
			this.ditherLabel = new System.Windows.Forms.Label();
			this.ditherSlider = new System.Windows.Forms.TrackBar();
			( (System.ComponentModel.ISupportInitialize)( this.thresholdSlider ) ).BeginInit();
			( (System.ComponentModel.ISupportInitialize)( this.thresholdUpDown ) ).BeginInit();
			( (System.ComponentModel.ISupportInitialize)( this.ditherUpDown ) ).BeginInit();
			( (System.ComponentModel.ISupportInitialize)( this.ditherSlider ) ).BeginInit();
			this.SuspendLayout();
			// 
			// thresholdSlider
			// 
			this.thresholdSlider.Location = new System.Drawing.Point( 0, 38 );
			this.thresholdSlider.Maximum = 255;
			this.thresholdSlider.Name = "thresholdSlider";
			this.thresholdSlider.Size = new System.Drawing.Size( 180, 45 );
			this.thresholdSlider.TabIndex = 2;
			this.thresholdSlider.TickFrequency = 32;
			this.thresholdSlider.Value = 1;
			this.thresholdSlider.ValueChanged += new System.EventHandler( this.thresholdSlider_ValueChanged );
			// 
			// thresholdLabel
			// 
			this.thresholdLabel.Location = new System.Drawing.Point( 6, 3 );
			this.thresholdLabel.Name = "thresholdLabel";
			this.thresholdLabel.Size = new System.Drawing.Size( 106, 33 );
			this.thresholdLabel.TabIndex = 0;
			this.thresholdLabel.Text = "&Transparency threshold:";
			// 
			// thresholdUpDown
			// 
			this.thresholdUpDown.Location = new System.Drawing.Point( 115, 14 );
			this.thresholdUpDown.Maximum = new decimal( new int[] {
            255,
            0,
            0,
            0} );
			this.thresholdUpDown.Name = "thresholdUpDown";
			this.thresholdUpDown.Size = new System.Drawing.Size( 56, 20 );
			this.thresholdUpDown.TabIndex = 1;
			this.thresholdUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.thresholdUpDown.Value = new decimal( new int[] {
            1,
            0,
            0,
            0} );
			this.thresholdUpDown.Enter += new System.EventHandler( this.thresholdUpDown_Enter );
			this.thresholdUpDown.ValueChanged += new System.EventHandler( this.thresholdUpDown_ValueChanged );
			this.thresholdUpDown.Leave += new System.EventHandler( this.thresholdUpDown_Leave );
			// 
			// preMultiplyAlphaCheckBox
			// 
			this.preMultiplyAlphaCheckBox.Location = new System.Drawing.Point( 8, 171 );
			this.preMultiplyAlphaCheckBox.Name = "preMultiplyAlphaCheckBox";
			this.preMultiplyAlphaCheckBox.Size = new System.Drawing.Size( 168, 24 );
			this.preMultiplyAlphaCheckBox.TabIndex = 6;
			this.preMultiplyAlphaCheckBox.Text = "&Multiply by alpha channel";
			this.preMultiplyAlphaCheckBox.CheckedChanged += new System.EventHandler( this.preMultiplyAlphaCheckBox_CheckedChanged );
			// 
			// useOriginalPaletteCheckBox
			// 
			this.useOriginalPaletteCheckBox.Location = new System.Drawing.Point( 8, 201 );
			this.useOriginalPaletteCheckBox.Name = "useOriginalPaletteCheckBox";
			this.useOriginalPaletteCheckBox.Size = new System.Drawing.Size( 168, 24 );
			this.useOriginalPaletteCheckBox.TabIndex = 7;
			this.useOriginalPaletteCheckBox.Text = "&Use original palette";
			this.useOriginalPaletteCheckBox.CheckedChanged += new System.EventHandler( this.useOriginalPaletteCheckBox_CheckedChanged );
			// 
			// ditherUpDown
			// 
			this.ditherUpDown.Location = new System.Drawing.Point( 115, 96 );
			this.ditherUpDown.Maximum = new decimal( new int[] {
            8,
            0,
            0,
            0} );
			this.ditherUpDown.Name = "ditherUpDown";
			this.ditherUpDown.Size = new System.Drawing.Size( 56, 20 );
			this.ditherUpDown.TabIndex = 4;
			this.ditherUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.ditherUpDown.Value = new decimal( new int[] {
            1,
            0,
            0,
            0} );
			this.ditherUpDown.Enter += new System.EventHandler( this.ditherUpDown_Enter );
			this.ditherUpDown.ValueChanged += new System.EventHandler( this.ditherUpDown_ValueChanged );
			this.ditherUpDown.Leave += new System.EventHandler( this.ditherUpDown_Leave );
			// 
			// ditherLabel
			// 
			this.ditherLabel.Location = new System.Drawing.Point( 6, 98 );
			this.ditherLabel.Name = "ditherLabel";
			this.ditherLabel.Size = new System.Drawing.Size( 106, 20 );
			this.ditherLabel.TabIndex = 3;
			this.ditherLabel.Text = "&Dithering level:";
			// 
			// ditherSlider
			// 
			this.ditherSlider.LargeChange = 2;
			this.ditherSlider.Location = new System.Drawing.Point( 0, 120 );
			this.ditherSlider.Maximum = 8;
			this.ditherSlider.Name = "ditherSlider";
			this.ditherSlider.Size = new System.Drawing.Size( 180, 45 );
			this.ditherSlider.TabIndex = 5;
			this.ditherSlider.Value = 1;
			this.ditherSlider.ValueChanged += new System.EventHandler( this.ditherSlider_ValueChanged );
			// 
			// PcxSaveConfigWidget
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 96F, 96F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add( this.useOriginalPaletteCheckBox );
			this.Controls.Add( this.preMultiplyAlphaCheckBox );
			this.Controls.Add( this.thresholdUpDown );
			this.Controls.Add( this.thresholdLabel );
			this.Controls.Add( this.thresholdSlider );
			this.Controls.Add( this.ditherUpDown );
			this.Controls.Add( this.ditherLabel );
			this.Controls.Add( this.ditherSlider );
			this.Name = "PcxSaveConfigWidget";
			this.Size = new System.Drawing.Size( 180, 229 );
			( (System.ComponentModel.ISupportInitialize)( this.thresholdSlider ) ).EndInit();
			( (System.ComponentModel.ISupportInitialize)( this.thresholdUpDown ) ).EndInit();
			( (System.ComponentModel.ISupportInitialize)( this.ditherUpDown ) ).EndInit();
			( (System.ComponentModel.ISupportInitialize)( this.ditherSlider ) ).EndInit();
			this.ResumeLayout( false );
			this.PerformLayout();

		}
		#endregion

		private void thresholdSlider_ValueChanged( object sender, System.EventArgs e )
		{
			if( this.thresholdUpDown.Value != (decimal)this.thresholdSlider.Value )
			{
				this.thresholdUpDown.Value = (decimal)this.thresholdSlider.Value;
			}

			UpdateToken();
		}

		private void thresholdUpDown_ValueChanged( object sender, System.EventArgs e )
		{
			if( this.thresholdSlider.Value != (int)this.thresholdUpDown.Value )
			{
				this.thresholdSlider.Value = (int)this.thresholdUpDown.Value;
			}
		}

		private void thresholdUpDown_Leave( object sender, System.EventArgs e )
		{
			thresholdUpDown_ValueChanged( sender, e );
		}

		private void thresholdUpDown_Enter( object sender, System.EventArgs e )
		{
			thresholdUpDown.Select( 0, thresholdUpDown.Text.Length );
		}

		private void useOriginalPaletteCheckBox_CheckedChanged( object sender, System.EventArgs e )
		{
			UpdateToken();
		}

		private void preMultiplyAlphaCheckBox_CheckedChanged( object sender, System.EventArgs e )
		{
			UpdateToken();
		}

		private void ditherSlider_ValueChanged( object sender, EventArgs e )
		{
			if( this.ditherUpDown.Value != (decimal)this.ditherSlider.Value )
			{
				this.ditherUpDown.Value = (decimal)this.ditherSlider.Value;
			}

			UpdateToken();
		}

		private void ditherUpDown_Enter( object sender, EventArgs e )
		{
			ditherUpDown.Select( 0, thresholdUpDown.Text.Length );
		}

		private void ditherUpDown_ValueChanged( object sender, EventArgs e )
		{
			if( this.ditherSlider.Value != (int)this.ditherUpDown.Value )
			{
				this.ditherSlider.Value = (int)this.ditherUpDown.Value;
			}
		}

		private void ditherUpDown_Leave( object sender, EventArgs e )
		{
			ditherUpDown_ValueChanged( sender, e );
		}
	}
}

