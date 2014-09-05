----------------------------------------------
PcxFileType : A file type plugin for Paint.NET
----------------------------------------------

History
-------
* 2006-10-30 - 0.9.0.0 - Original release
* 2006-12-24 - 0.9.1.0 - Added "Use original palette" option
* 2007-01-25 - 0.9.2.0 - Fixed RLE compression writing
* 2007-10-21 - 0.9.3.0 - Force 1-bit images to black/white palette
* 2014-06-24 - 0.9.4.0 - Compatability with Paint.NET 4.0

Summary
-------

This is a file type plugin for Paint.NET, which supports
the ZSoft Corporation PC Paintbrush "PCX" image format.

Supported are:

* Loading 1-, 2-, 4-, 8- and 24-bit color PCX files 
* Saving 4- and 8-bit PCX files 


References
----------

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

One more option is present but only has an effect if the image
was originally loaded as a palettized PCX image:

* Use original palette

If selected, the original palette will be used when saving.

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
license. See License.txt for more information.


Feedback & Bug Reporting
------------------------

Please send feedback to: inexorabletash@gmail.com

