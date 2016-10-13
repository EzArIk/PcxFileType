/////////////////////////////////////////////////////////////////////////////////
// PCX Plugin for Paint.NET
// Copyright (C) 2006 Joshua Bell (inexorabletash@gmail.com)
// Portions Copyright (C) 2006 Rick Brewster, et. al.
// See License.txt for complete licensing and attribution information.
/////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////////////////////////
//Modified by EzArIk(Thomas C. Maylam [on: (GB)11/20/2016]) to support the use of preset Pallettes.//
/////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PaintDotNet;
using System.IO;


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
        private CheckBox preset_palette_check_box;
        private Panel panel1;
        private ComboBox preset_palettes_combo_box;
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

        //'Global' Widgit Strings:
        string[] import_pal, import_name;
        //string preset_palette_sender;
        //'Global' Widgit Strings:

        void load_set_palette()
        {
           // FileStream import_pal_file;
            StreamReader test_stream_reader;
            // string pal_path = "";
            string in_line = "";
            int import_maximum = 0, import_count = 0;
            //bool did_it_work = false;


            //////////////
            //Important!//
            //////////////

            int found_key = -1;

         //   pal_path = GetCurrentDirectory();
            try {
                
                using (test_stream_reader = new StreamReader("FileTypes/Preset_Pallettes.pst_pal.txt"))
                {
                    import_count = 0;
                    import_maximum = 1;
                   // ((PcxSaveConfigToken)this.Token).preset_palette_string = new string[1];
                    import_pal = new string[1];
                    import_name = new string[1];

                    // Get the start of the files list, this part defines how many entries to look for.

                    while ((in_line = test_stream_reader.ReadLine()) != null)
                     {
                       
                        if (in_line == "NUM" && found_key == -1)
                        {
                            // this line tells the program that the next row in the file should contain the maximum number of entries:
                            found_key = 0;
                            continue;
                        }

                        if (found_key == 0)
                        {
                            // this line gets the number in the next row and sets it as the maximum value for entries:
                            import_maximum = Convert.ToInt32(in_line);
                           // ((PcxSaveConfigToken)this.Token).preset_palette_string = new string[import_maximum];
                            import_pal = new string[import_maximum];
                            import_name = new string[import_maximum];
                            found_key = 1;
                        }

                        if (in_line == "ENT" && found_key == 1)
                        {
                            //Looks For the entry Flag and progresses to the next stage it it finds it.
                            found_key = 2;
                            continue;
                        }

                        if (found_key == 2)
                        {
                            import_name[import_count] = in_line;
                            preset_palettes_combo_box.Items.Insert(import_count, import_name[import_count]);
                            preset_palettes_combo_box.SelectedIndex = 0;
                            //update_preset_palette_string_token();
                            found_key = 3;
                            continue;
                        }


                        if (found_key == 3)
                        {
                            import_pal[import_count] = in_line;
                            found_key = 1;
                            import_count++;
                        }

                        if (import_count >= import_maximum)
                        {
                            break;
                        }

                    }
   
                     if (in_line == null)
                    {
                         throw new ArgumentNullException();
                    }

                }

            }
            catch (Exception e) {
                //    did_it_work = false;
                preset_palette_check_box.Enabled = false;
                preset_palettes_combo_box.Text = "Error:"+ e;
                //preset_palettes_combo_box.ValueMember.Remove(0, preset_palettes_combo_box.ValueMember.Length);
            }

            //if (did_it_work == true) {

            //            }
            //import_pal;
            //((PcxSaveConfigToken)this.Token).preset_palette_string = "4278190080,4279894016,4281535232,4282843911,4286513152,4286578688,4288348160,4294901760,4294934592,4294927460,4294934399,4294937226,4293248421,4294950847,4293445832,4294888410,4294960099,4294638311,4289013286,4293092452,4290139428,4288355072,4291990162,4293178246,4292590519,4293059288,4285081861,4291792751,4292585855,4294572243,4293388263,4283568915,4283967748,4290143560,4291793008,4286595072,4290147912,4291529547,4291868833,4281473801,4283183616,4285018377,4291404401,4291472010,4292006329,4279833861,4281873415,4287714596,4288371483,4289762851,4288713562,4293059266,4283714565,4284634112,4285165568,4286545664,4287137800,4288387867,4290097004,4291217776,4291341457,4292269220,4284638476,4285372708,4286950726,4287740744,4289571717,4291085230,4278195968,4278200064,4281223217,4281956608,4280640805,4285377060,4288141395,4290174061,4289973679,4292664476,4278883594,4285377096,4287220105,4289393578,4293132259,4278216192,4283157323,4287688591,4291677905,4285396589,4290888123,4290167500,4278218029,4283617681,4278197276,4290049780,4278206528,4279080084,4278220149,4282943624,4285376689,4286488254,4288138964,4291946739,4289967827,4291219441,4278519064,4279061901,4286558429,4288131788,4293323514,4279045681,4278784594,4281627827,4283394524,4287006406,4285641201,4278190195,4278782356,4282469829,4285230064,4281412572,4281944028,4287663611,4289442810,4289843701,4278979839,4278223103,4278222783,4288074488,4279908954,4285198840,4278255615,4286644223,4279645750,4284078553,4279979129,4281162119,4281364367,4280557081,4281545805,4280894803,4280427106,4281750641,4281939599,4283060371,4284443813,4285811640,4280025116,4283183440,4282983314,4284950188,4287780315,4287784667,4291134975,4283501905,4287780279,4285410267,4294338801,4292516314,4291933899,4294890750,4294837502,4294960633,4281413677,4285468784,4290000052,4294940927,4287760786,4294884314,4294939089,4285408072,4289539437,4282723610,4283123755,4285082664,4287714891,4289755250,4281936661,4286006555,4286598946,4294356336,4285025330,4294306988,4294966980,4291213347,4290687546,4293980265,4291744803,4294966923,4288387840,4290174024,4286085176,4287335243,4282989618,4282936612,4285028947,4279769112,4290290572,4285822800,4283322190,4287137649,4289891193,4289504919,4287598735,4289834928,4284179045,4285492847,4288062635,4285954190,4283259757,4287072170,4286669121,4287261293,4285372781,4285370440,4282936903,4290178413,4282614595,4287746852,4290178376,4294934028,4288355099,4281109291,4294901887,4278255488,4292048084,4282978962,4286578943,4282978999,4290218459,4285961368,4285871477,4281167213,4287784594,4289695631,4290349728,4291139504,4290740159,4294235852,4283570437,4294940037,4294954389,4294945791,4294952371,4294940017,4294953408,4294967295,4294908415,4286388735,4279893885,4294908185,4294967052,4279893785,4279835135,4294933785,4294111986,4294902015";
            

        }

        void update_preset_palette_string_token(){
            ((PcxSaveConfigToken)this.Token).preset_palette_string = import_pal[preset_palettes_combo_box.SelectedIndex];
        }

        protected override void InitTokenFromWidget()
		{
			( (PcxSaveConfigToken)this.Token ).Threshold = this.thresholdSlider.Value;
			( (PcxSaveConfigToken)this.Token ).DitherLevel = this.ditherSlider.Value;
			( (PcxSaveConfigToken)this.Token ).PreMultiplyAlpha = this.preMultiplyAlphaCheckBox.Checked;
			( (PcxSaveConfigToken)this.Token ).UseOriginalPalette = this.useOriginalPaletteCheckBox.Checked;
            ( (PcxSaveConfigToken)this.Token ).preset_palette = this.preset_palette_check_box.Checked;
            ( (PcxSaveConfigToken)this.Token ).preset_palette_arr_position = this.preset_palettes_combo_box.SelectedIndex;
            if(import_pal != null) ( (PcxSaveConfigToken)this.Token ).preset_palette_string = this.import_pal[this.preset_palettes_combo_box.SelectedIndex];
            //preset_palette_sender
        }

		protected override void InitWidgetFromToken( PaintDotNet.SaveConfigToken token )
		{
			this.thresholdSlider.Value = ( (PcxSaveConfigToken)token ).Threshold;
			this.ditherSlider.Value = ( (PcxSaveConfigToken)token ).DitherLevel;
			this.preMultiplyAlphaCheckBox.Checked = ( (PcxSaveConfigToken)token ).PreMultiplyAlpha;
			this.useOriginalPaletteCheckBox.Checked = ( (PcxSaveConfigToken)token ).UseOriginalPalette;
            this.preset_palette_check_box.Checked = ( (PcxSaveConfigToken)token ).preset_palette;

            try
            {
                this.preset_palettes_combo_box.SelectedIndex = ((PcxSaveConfigToken)this.Token).preset_palette_arr_position;
            }catch { }
            //((PcxSaveConfigToken)this.Token).preset_palette_string = import_pal[((PcxSaveConfigToken)this.Token).preset_palette_arr_position];
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
            this.preset_palette_check_box = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.preset_palettes_combo_box = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ditherUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ditherSlider)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // thresholdSlider
            // 
            this.thresholdSlider.Location = new System.Drawing.Point(0, 35);
            this.thresholdSlider.Maximum = 255;
            this.thresholdSlider.Name = "thresholdSlider";
            this.thresholdSlider.Size = new System.Drawing.Size(180, 45);
            this.thresholdSlider.TabIndex = 2;
            this.thresholdSlider.TickFrequency = 32;
            this.thresholdSlider.Value = 1;
            this.thresholdSlider.ValueChanged += new System.EventHandler(this.thresholdSlider_ValueChanged);
            // 
            // thresholdLabel
            // 
            this.thresholdLabel.Location = new System.Drawing.Point(6, 3);
            this.thresholdLabel.Name = "thresholdLabel";
            this.thresholdLabel.Size = new System.Drawing.Size(106, 31);
            this.thresholdLabel.TabIndex = 0;
            this.thresholdLabel.Text = "&Transparency threshold:";
            // 
            // thresholdUpDown
            // 
            this.thresholdUpDown.Location = new System.Drawing.Point(115, 14);
            this.thresholdUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.thresholdUpDown.Name = "thresholdUpDown";
            this.thresholdUpDown.Size = new System.Drawing.Size(56, 20);
            this.thresholdUpDown.TabIndex = 1;
            this.thresholdUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.thresholdUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.thresholdUpDown.ValueChanged += new System.EventHandler(this.thresholdUpDown_ValueChanged);
            this.thresholdUpDown.Enter += new System.EventHandler(this.thresholdUpDown_Enter);
            this.thresholdUpDown.Leave += new System.EventHandler(this.thresholdUpDown_Leave);
            // 
            // preMultiplyAlphaCheckBox
            // 
            this.preMultiplyAlphaCheckBox.Location = new System.Drawing.Point(8, 146);
            this.preMultiplyAlphaCheckBox.Name = "preMultiplyAlphaCheckBox";
            this.preMultiplyAlphaCheckBox.Size = new System.Drawing.Size(168, 20);
            this.preMultiplyAlphaCheckBox.TabIndex = 6;
            this.preMultiplyAlphaCheckBox.Text = "&Multiply by alpha channel";
            this.preMultiplyAlphaCheckBox.CheckedChanged += new System.EventHandler(this.preMultiplyAlphaCheckBox_CheckedChanged);
            // 
            // useOriginalPaletteCheckBox
            // 
            this.useOriginalPaletteCheckBox.Location = new System.Drawing.Point(8, 163);
            this.useOriginalPaletteCheckBox.Name = "useOriginalPaletteCheckBox";
            this.useOriginalPaletteCheckBox.Size = new System.Drawing.Size(168, 21);
            this.useOriginalPaletteCheckBox.TabIndex = 7;
            this.useOriginalPaletteCheckBox.Text = "&Use original palette";
            this.useOriginalPaletteCheckBox.CheckedChanged += new System.EventHandler(this.useOriginalPaletteCheckBox_CheckedChanged);
            // 
            // ditherUpDown
            // 
            this.ditherUpDown.Location = new System.Drawing.Point(115, 81);
            this.ditherUpDown.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.ditherUpDown.Name = "ditherUpDown";
            this.ditherUpDown.Size = new System.Drawing.Size(56, 20);
            this.ditherUpDown.TabIndex = 4;
            this.ditherUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ditherUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ditherUpDown.ValueChanged += new System.EventHandler(this.ditherUpDown_ValueChanged);
            this.ditherUpDown.Enter += new System.EventHandler(this.ditherUpDown_Enter);
            this.ditherUpDown.Leave += new System.EventHandler(this.ditherUpDown_Leave);
            // 
            // ditherLabel
            // 
            this.ditherLabel.Location = new System.Drawing.Point(6, 83);
            this.ditherLabel.Name = "ditherLabel";
            this.ditherLabel.Size = new System.Drawing.Size(106, 18);
            this.ditherLabel.TabIndex = 3;
            this.ditherLabel.Text = "&Dithering level:";
            // 
            // ditherSlider
            // 
            this.ditherSlider.LargeChange = 2;
            this.ditherSlider.Location = new System.Drawing.Point(0, 102);
            this.ditherSlider.Maximum = 8;
            this.ditherSlider.Name = "ditherSlider";
            this.ditherSlider.Size = new System.Drawing.Size(180, 45);
            this.ditherSlider.TabIndex = 5;
            this.ditherSlider.Value = 1;
            this.ditherSlider.ValueChanged += new System.EventHandler(this.ditherSlider_ValueChanged);
            // 
            // preset_palette_check_box
            // 
            this.preset_palette_check_box.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.preset_palette_check_box.ForeColor = System.Drawing.SystemColors.ControlText;
            this.preset_palette_check_box.Location = new System.Drawing.Point(5, 2);
            this.preset_palette_check_box.Name = "preset_palette_check_box";
            this.preset_palette_check_box.Size = new System.Drawing.Size(166, 19);
            this.preset_palette_check_box.TabIndex = 8;
            this.preset_palette_check_box.Text = "&Use Preset Palettes";
            this.preset_palette_check_box.UseVisualStyleBackColor = false;
            this.preset_palette_check_box.CheckedChanged += new System.EventHandler(this.preset_palette_check_box_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.panel1.Controls.Add(this.preset_palettes_combo_box);
            this.panel1.Controls.Add(this.preset_palette_check_box);
            this.panel1.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.panel1.Location = new System.Drawing.Point(3, 183);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(174, 45);
            this.panel1.TabIndex = 10;
            // 
            // preset_palettes_combo_box
            // 
            this.preset_palettes_combo_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.preset_palettes_combo_box.FormattingEnabled = true;
            this.preset_palettes_combo_box.Location = new System.Drawing.Point(6, 21);
            this.preset_palettes_combo_box.Name = "preset_palettes_combo_box";
            this.preset_palettes_combo_box.Size = new System.Drawing.Size(162, 21);
            this.preset_palettes_combo_box.TabIndex = 9;
            this.preset_palettes_combo_box.SelectedIndexChanged += new System.EventHandler(this.preset_palettes_combo_box_SelectedIndexChanged);
            // 
            // PcxSaveConfigWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.useOriginalPaletteCheckBox);
            this.Controls.Add(this.preMultiplyAlphaCheckBox);
            this.Controls.Add(this.thresholdUpDown);
            this.Controls.Add(this.thresholdLabel);
            this.Controls.Add(this.thresholdSlider);
            this.Controls.Add(this.ditherUpDown);
            this.Controls.Add(this.ditherLabel);
            this.Controls.Add(this.ditherSlider);
            this.Name = "PcxSaveConfigWidget";
            this.Size = new System.Drawing.Size(180, 230);
            this.Load += new System.EventHandler(this.PcxSaveConfigWidget_Load);
            ((System.ComponentModel.ISupportInitialize)(this.thresholdSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ditherUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ditherSlider)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
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
            if (useOriginalPaletteCheckBox.Checked == true) preset_palette_check_box.Checked = false;
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

        private void PcxSaveConfigWidget_Load(object sender, EventArgs e)
        {
            load_set_palette();
        }

        private void preset_palette_check_box_CheckedChanged(object sender, EventArgs e)
        {
           if (preset_palette_check_box.Checked == true) useOriginalPaletteCheckBox.Checked = false;
            UpdateToken();
        }

        private void preset_palettes_combo_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            //update_preset_palette_string_token();
            UpdateToken();
        }
    }
}

