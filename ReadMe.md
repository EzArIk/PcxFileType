PcxFileType : A file type plugin for Paint.NET
----------------------------------------------

History
-------
* 2006-10-30 - 0.9.0.0 - Original release
* 2006-12-24 - 0.9.1.0 - Added "Use original palette" option
* 2007-01-25 - 0.9.2.0 - Fixed RLE compression writing
* 2007-10-21 - 0.9.3.0 - Force 1-bit images to black/white palette
* 2014-06-24 - 0.9.4.0 - Compatibility with Paint.NET 4.0
* 2015-01-09 - 0.9.5.0 - Build for .NET Framework 4.5
* 2016-10-13 - 0.9.5.1 - [EzArIk]: Added (somewhat crude but effective) pre-set palette support, 
  included example pre-set file, (also made the project reference Paint.net Api automatically.)

Summary
-------
This is a file type plugin for [Paint.NET](http://www.getpaint.net/), 
which adds support for the ZSoft Corporation PC Paintbrush "PCX" image
format.

Supported are:

* Loading 1-, 2-, 4-, 8- and 24-bit color PCX files 
* Saving 4- and 8-bit PCX files 

Paint.NET Forum Topic:

* http://forums.getpaint.net/index.php?/topic/2135-pcx-plug-in/

PCX File Format References
--------------------------
* http://en.wikipedia.org/wiki/PCX
* http://www.qzx.com/pc-gpe/pcx.txt
* http://www.fileformat.info/format/pcx/
* http://courses.ece.uiuc.edu/ece390/books/labmanual/graphics-pcx.html

Loading
-------
The following image formats can be loaded:

* PCX Versions 2.5, 2.8 and 3.0 
* 1, 2, 4 and 8-bit (2, 4, 16 and 256-color) palettized images
* 24-bit "true color" images
* Run-length encoded and unencoded
* 2-, 4-, and 16-color default palettes (monochrome, CGA, EGA)
* 16-color and 256-color custom palettes
* multiple bit planes (common for 4-bit and required for 24-bit)

Saving
------
Saving as PCX always attempts to create an 8-bit (256-color) 
palettized image, although if the palette ends up being 16 colors 
or less a 4-bit palette is used instead. Saving as 24-bit PCX is
not supported.

The same options are presented as for Paint.NET's GIF file type 
support:

* Transparency threshold
* Dithering level
* Multiply by alpha channel

The PCX format does not support transparency, but the transparency
threshold can be used to adjust how tranparency present in the image
is applied to the pixels before the image is flattened.

Two more options are present but one only has an effect if the image
was originally loaded as a palettized PCX image:

* Use original palette

If selected, the original palette will be used when saving.

The second option is: 
* Use Pre-set palette

If selected, the images palette can be overridden with one of the pre-set palettes stored in Preset_Pallettes.pst_pal.txt, 
these can be changed by editing that text file, for the current official Preset_Pallettes.pst_pal.txt look in the folder 
named: _Preset_Palettes_Example_File (a readme in that folder describes where to place the palette file to use it.)

Installation
------------
Place the PcxFileType.DLL file in the the 'FileTypes' folder in your 
Paint.NET installation location. For example, in:

    C:\Program Files\Paint.NET\FileTypes

The new file type will be available when you next start Paint.NET.

To open PCX files in Paint.NET by default, right-click a PCX file
in Windows Explorer, select "Open With" then "Choose Program...",
select Paint.NET (you may have to click "Browse...") and then check 
"Always use the selected program to open this kind of file"


License
-------
This work is distributed under the terms and conditions of the MIT 
license. See License.md for more information.


Feedback & Bug Reporting
------------------------
Bug reports can be filed at:

https://github.com/inexorabletash/PcxFileType/issues

