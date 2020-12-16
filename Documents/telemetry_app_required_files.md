# Arrabona Racing Team Telemetry Software

- [Arrabona Racing Team Telemetry Software](#arrabona-racing-team-telemetry-software)
  - [Required files](#required-files)
    - [maps.csv](#mapscsv)
    - [drivers.csv](#driverscsv)
    - [groups.csv](#groupscsv)

## Required files

### maps.csv

Contains all the data of the saved maps.

**name**, **start point** coordinate and **year**.

```csv
map_name;start_point_x;start_point_y;year
FSEast;93.95;448.4;2019
```

### drivers.csv

Contains all the pre saved **drivers names**.

### groups.csv

Contains all the pre saved **groups** with **attributes**.

```csv
group_name;group_attributes
EngineMapping;rev,ath,lam,ign_1,ti_1,gear,speed
```
