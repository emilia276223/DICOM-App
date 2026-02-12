[🇵🇱 Polski](README.md) | 🇬🇧 English

# Application for viewing and editing DICOM images with AI plugin support

The application was developed as part of an engineering thesis at the University of Wrocław. It was created to simplify and automate measurements of optic nerve width.

## Application Installation

### System Requirements

The application runs on both Windows and Linux.

It requires the `.NET 9.0` runtime to be installed.
You can download it from:

`dotnet.microsoft.com/en-us/download/dotnet/9.0`

After installing `.NET 9.0`, the application can be launched.

---

### Installation via Package Download

The application package is available at:

`github.com/emilia276223/DICOM-App/releases/tag/v1.0.0`

1. Download `publish.zip`
2. Extract the archive
3. Run `DICOMApp.exe` from the `publish` directory

If `.NET 9.0` is not installed, the user will be redirected to the download page when launching the application for the first time.

---

### Installation and Running from Source Code

The source code is available at:

`github.com/emilia276223/DICOM-App`

You can download the ZIP archive or clone the repository:

```bash
git clone https://github.com/emilia276223/DICOM-App
cd DICOM-App
```

Then run:

```
dotnet run
```

The application requires `.NET 9.0`, available at: `dotnet.microsoft.com/en-us/download/dotnet/9.0`

---

## Application Features

1. Opening DICOM files
2. Marking points on images
3. Saving markings to a database
4. Enabling image magnification and marking points on the zoomed view
5. Exporting anonymized measurement data from the database
6. Viewing measurement trends for a selected patient
7. Connecting an AI model to perform automatic markings

[User Guide](README.user_guide_en.md)

---

## Documentation

Documentation is available at:

https://emilia276223.github.io/DICOM-App/
