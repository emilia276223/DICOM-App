# User Manual

When the application is launched, it opens the main window shown in the image below. It contains buttons that allow you to use the functionalities provided by the program.

![Main application screen](images/image.png)

## Importing a DICOM image from a file

The application allows importing a DICOM image. After clicking the `Importuj nowy obraz` button, a file explorer window appears. After selecting a DICOM file in this window, it is imported into the application and then displayed. The image below shows an example view after importing an image. On the left side, the imported image appears, and in the middle section at the top, the patient’s first and last name and the study date are displayed.

![Application screen after image import](images/main_view_z_obrazem.png)

The displayed DICOM image is cropped so that the ultrasound imaging is fully visible, but the burned-in metadata on the image is not shown. Examples of the original and cropped images are shown in the figures below.

![Cropped image](images/000021_cropped.jpeg)
![Original image](images/000021.jpeg)

## Importing a directory of DICOM images

The program allows importing a selected directory of DICOM images. To do this, click the `Importuj katalog obrazów` button. After clicking the button, a file explorer window opens. You need to select the appropriate directory, and the application will import all DICOM files from it. At the end of the import process, the application displays information like in the image below.

![Notification about completed DICOM directory import](images/finished_import.png)

## Opening an imported image

If images have been imported into the application (using `Importuj nowy obraz` or `Importuj katalog obrazów`), they can be opened by selecting the patient and study date. Clicking the `Wybierz pacjenta` button opens a list of patients whose images have been imported into the application, as shown below:

![List of patients to choose from](images/opened_patients_list.png)

After selecting a patient from the list (by clicking one of the options), the `Wybierz badanie` button is unlocked. Clicking it opens a list of studies dates for the selected patient that have been imported. In a similar way, you can select the study from which the image should be opened. The study dates are formatted as `yyyy.mm.dd`. The figure below shows an example view while choosing an study:

![Choosing a study for a given patient](images/choosing_study.png)

When an study is selected, the `Wybierz obraz` button is unlocked. Clicking this button opens a list of images from the selected study of the given patient. After selecting an image, it is displayed. The application window then looks like:

![Application window after selecting an image to open](images/opened_image.png)

As seen in the figure above, when an image is opened this way, the `Poprzedni obraz` and `Następny obraz` buttons appear below it. These allow easy navigation between images from the same study.

The `Wybierz pacjenta`, `Wybierz badanie`, `Wybierz obraz` buttons allow changing the selected values. If the selected patient is changed, the `Wybierz obraz` button will be locked until an study is selected.

## Drawing lines on the image

![Selected option: marking](images/radio_button_zaznaczanie.png)

If the `Zaznaczanie` option is selected (as shown in the figure above), it is possible to mark lines on the image. You can draw lines by pressing the left mouse button at the point that should be the start of the line and releasing it at the endpoint. There are two lines available – red and blue – which are drawn alternately.

During drawing, the lengths of the lines are updated in real time. They can be seen in the middle part of the screen. Example of line marking:

![Drawing lines and their lengths](images/drawing_lines.png)

## Zooming in on the image

![Selected zoom option](images/radio_button_lupa.png)

The application allows zooming in on a selected part of the image. To do this, switch to the `Lupa` mode as shown in the figure above. After switching the mode on the main image, you can select the part that should be zoomed in. When holding the left mouse button at a selected point on the image, the zoomed fragment is set. The point where the mouse is located on the main image is at the center of the zoomed fragment. Moving the mouse (while pressed) on the main image allows moving the zoomed fragment.

After selecting the appropriate zoomed fragment, you can mark lines on it. This allows for more precise marking. The lengths of the lines are displayed in the same place as for lines on the main image. Lines are also reflected in real time on the main image:

![Zoomed image and marked lines](images/lupa_i_odpowiadanie_sobie_linii.png)

## Saving marked lines

After marking lines on the image (main or zoomed), you can save this marking to the database using the `Zapisz zaznaczenie` button.

## Exporting saved data

The `Eksport Danych` button starts exporting data saved in the application. A directory is created on the Desktop containing saved PNG image files and CSV files with information exported from the database. The data exported by the application is fully anonymized. The exported .ZIP directory is also saved on the Desktop.

## Chart showing changes over time in optic nerve width

When opening an image in the application, a chart may be displayed. If there is at least one marking saved in the database for images of the patient whose ultrasound image is being opened, a chart is shown. The chart shows the measured optic nerve width values. The X-axis of the chart shows the dates of the studies from which these values come. This allows reviewing changes in optic nerve width over time for a given patient. Example chart:

![Example chart](images/chart_example.png)

The chart is displayed on the right side of the application window. If the database does not contain markings that allow creating a chart, a message is displayed instead: `W bazie danych aplikacji nie ma zaznaczeń na obrazach z badań wybranego pacjenta`.

## Using Artificial Intelligence in the application

The application allows using an AI model to predict points used to determine the optic nerve width. To enable AI usage, select `Automatyczne zaznaczanie linii`:

![Automatic line marking by AI model](images/automatyczne_zaznaczanie_linii.png)

From that moment, when images are opened, the AI model's prediction will be automatically marked on them. It will be displayed using lines drawn on the image.

The program allows uploading your own AI model. The `Zmień model AI` button opens the file explorer and allows selecting a `.ONNX` file, which will be imported into the application. If the uploaded model meets the [technical requirements](README.MLIO_en.md), it will be used in the application for point prediction from that moment. Models can be uploaded multiple times, which allows using different models.
