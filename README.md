# Viewpoint Systems SVN Toolkit for LabVIEW NXG

The SVN Toolkit provides beta integration to SVN from within the LabVIEW NXG project files pane. One can view the SVN status of files in your project and perform SVN actions in the project files pane with one click of a button. The most common actions are now available as right-click menu options on a file selected in the project files pane. The SVN Toolkit is an open-source project managed here.

## Getting Started

1) Install the Tortoise SVN client on your machine: https://tortoisesvn.net/downloads.html
2) Install the Viewpoint Systems SVN Toolkit for LabVIEW NXG from NI Package Manager, located in the Product Add-Ons section
3) Create a TSVN repository from an empty folder on disk (right-click on a new folder> TortoiseSVN > Create repository here)
4) Right-click on a LabVIEW NXG project folder and TortoiseSVN > Import it to your repo
5) Check out that project from your repository to a folder on disk
6) Open the project in LabVIEW NXG and the Toolkit should reflect the status of every file you checked out in the project files pane

### Prerequisites

-TortoiseSVN or other SVN client
-Viewpoint Systems SVN Toolkit for LabVIEW NXG

#### Contributing

In this initial beta version, the most common actions are available as right-click menu options within LabVIEW NXG but there are additional features to be implemented, such as account management or handling file moves. Please submit a pull request with the changes you'd like to incorporate.
