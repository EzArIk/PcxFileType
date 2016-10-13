/////////////////////////////////////////////////////////////////////////////////
// PCX Plugin for Paint.NET
// Copyright (C) 2006 Joshua Bell (inexorabletash@gmail.com)
// Portions Copyright (C) 2006 Rick Brewster, et. al.
// See License.txt for complete licensing and attribution information.
/////////////////////////////////////////////////////////////////////////////////

using System;
using PaintDotNet;

namespace PcxFileTypePlugin
{
    [Serializable]
    public class PcxSaveConfigToken
        : SaveConfigToken
    {
        public override object Clone()
        {
            return new PcxSaveConfigToken(this);
        }

        public PcxSaveConfigToken(int threshold, bool preMultiplyAlpha, bool useOriginalPalette, int ditherLevel, bool rle, bool preset_palette, string preset_palette_string, int preset_palette_arr_position)
        {
            this.threshold = threshold;
            this.preMultiplyAlpha = preMultiplyAlpha;
            this.useOriginalPalette = useOriginalPalette;
            this.ditherLevel = ditherLevel;
            this.rle = rle;
            this.preset_palette = preset_palette;
            this.preset_palette_string = preset_palette_string;
            this.preset_palette_arr_position = preset_palette_arr_position;
            Validate();
        }

        protected PcxSaveConfigToken(PcxSaveConfigToken copyMe)
        {
            this.threshold = copyMe.threshold;
            this.preMultiplyAlpha = copyMe.preMultiplyAlpha;
            this.useOriginalPalette = copyMe.useOriginalPalette;
            this.ditherLevel = copyMe.ditherLevel;
            this.rle = copyMe.rle;
            this.preset_palette = copyMe.preset_palette;
            this.preset_palette_string = copyMe.preset_palette_string;
            this.preset_palette_arr_position = copyMe.preset_palette_arr_position;
        }

        private const int minThreshold = 0;
        private const int maxThreshold = 255;
        private const int minDitherLevel = 0;
        private const int maxDitherLevel = 8;

        private int threshold;
        private bool preMultiplyAlpha;
        private bool useOriginalPalette;
        private int ditherLevel;
        private bool rle;

        //----New-Pal-Stuff------
        public bool preset_palette;
        public string/*[]*/ preset_palette_string;
        public int preset_palette_arr_position;
        //----New-Pal-Stuff------

        public int Threshold
        {
            get
            {
                return threshold;
            }

            set
            {
                if (value < minThreshold || value > maxThreshold)
                    throw new ArgumentOutOfRangeException("threshold must be " + minThreshold + " to " + maxThreshold + ", inclusive");

                this.threshold = value;
            }
        }
        public bool PreMultiplyAlpha
        {
            get
            {
                return this.preMultiplyAlpha;
            }

            set
            {
                this.preMultiplyAlpha = value;
            }
        }
        public bool UseOriginalPalette
        {
            get
            {
                return this.useOriginalPalette;
            }

            set
            {
                this.useOriginalPalette = value;
            }
        }
        public int DitherLevel
        {
            get
            {
                return this.ditherLevel;
            }

            set
            {
                if (value < minDitherLevel || value > maxDitherLevel)
                    throw new ArgumentOutOfRangeException("ditherLevel must be " + minDitherLevel + " to " + maxDitherLevel + ", inclusive");

                this.ditherLevel = value;
            }
        }
        public bool RleCompress
        {
            get
            {
                return this.rle;
            }

            set
            {
                this.rle = value;
            }
        }

       /* public bool preset_palette
        {
            get
            {
                return this.preset_palette;
            }

            set
            {
                this.preset_palette = value;
            }
        }*/


        public override void Validate()
        {
            if (this.threshold < minThreshold || this.threshold > maxThreshold)
                throw new ArgumentOutOfRangeException(String.Format("threshold must be {0} to {1}, inclusive", minThreshold, maxThreshold));

            if (this.ditherLevel < minDitherLevel || this.ditherLevel > maxDitherLevel)
                throw new ArgumentOutOfRangeException(String.Format("ditherLevel must be {0} to {1}, inclusive", minDitherLevel, maxDitherLevel));
        }
    }
}
