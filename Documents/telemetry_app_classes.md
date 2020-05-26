# Arrabona Racing Team Telemetry Software

## Classes and User Controls

- **Charts**
  - *Classes*
    - ChartBuilder.cs
    - ChartLineColors.cs
    - KalmanFilter.cs
  - *User Controls*
- **Datas**
  - *Classes*
    - Data.cs
    - DataReader.cs
    - InputFile.cs
    - LineSeriesOption.cs
  - *User Controls*
    - DatasMenuContent.xaml
    - InputFileListElement.xaml
- **Icons**
  - art.ico
  - art_logo_jBd_icon.ico
- **Laps**
  - *Classes*
    - Lap.cs
  - *User Controls*
    - AllLapChannels.xaml
    - AllLapListElement.xaml
    - LapChannels.xaml
    - LapListElement.xaml
    - LapReportContent.xaml
    - LapsContent.xaml
- **Maps**
  - *Classes*
  - *User Controls*
    - MapEditor.xaml
- **Pilots**
  - *Classes*
    - Pilot.cs
    - PilotManager.cs
  - *User Controls*
    - PilotContentTab.xaml
    - PilotSettings.xaml
    - PilotsMenuContent.xaml
- **Settings**
  - *Classes*
  - *User Controls*
    - SettingsMenuContent.xaml
- **Tabs**
  - *Classes*
    - TabManager.cs
  - *User Controls*
- **Tracks**
  - *Classes*
  - *User Controls*
    - TrackContent.xaml
- App.config
- App.xaml
- ART_design.xaml
- ART_tabblz_design.xaml
- ButtonCommands.cs
- GGDiagram_UC.xaml
- MainWindow.xaml
- packages.config

<!-- pagebreak -->

> ### ChartBuilder.cs
>
> Builds charts.

> ### ChartLineColors.cs
>
> Contains a colors list.

> ### KalmanFilter.cs
>
> Kalman filter.
> The value of *Q* is the sensitivity of the filter. If *Q* the smaller is, the more sensitive will be the filter.
>

> ### Data.cs
>
> Contains a single data from **InputFile**.

> ### DataReader.cs
>
> Reads a file into a **Pilot**.

> ### InputFile.cs
>
> Contains all the **Data** to a **Pilot** from a file.

> ### LineSeriesOption.cs
>
> Contains a stroke thickness and a stroke color.

> ### DatasMenuContent.xaml
>
> The content of the *Diagrams* tab.

> ### InputFileListElement.xaml
>
> Rrepresents an input file of a **Pilot**.

> ### Lap.cs
>
> Contatins all data to a lap.

> ### AllLapChannels.xaml
>
> The content of the *All laps* lap on the Pilots *Laps* tab.

> ### AllLapListElement.xaml
>
> Rrepresents the *All laps* lap on the Pilots *Laps* tab.

> ### LapChannels.xaml
>
> The content of the *laps* lap on the Pilots *Laps* tab.

> ### LapListElement.xaml
>
> Rrepresents the *laps* lap on the Pilots *Laps* tab.

> ### LapReportContent.xaml
>
> The content of the lap report.

> ### LapsContent.xaml
>
> The content of a *Pilot* tab.

> ### MapEditor.xaml
>
> You can edit the input file's map.

> ### Pilot.cs
>
> Contains all the **InputFile**s to a pilot.

> ### PilotManager.cs
>
> Contains all the **Pilot**s.

> ### PilotContentTab.xaml
>
> Contains all the tabs to a pilot.

> ### PilotSettings.xaml
>
> Rrepresents a pilot in the *Pilots* tab.

> ### PilotsMenuContent.xaml
>
> Rrepresents the content of the *Pilots* tab.

> ### SettingsMenuContent.xaml
>
> Rrepresents the content of the *Settings* tab.

> ### TabManager.cs
>
> Contains all the tab items of the menu.

> ### TrackContent.xaml
>
> Rrepresents the content of the *Track* tab.
