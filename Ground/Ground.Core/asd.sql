CREATE TABLE raumschiff
(
    RaumschiffNr   int     NOT NULL,
    RaumschiffName VARCHAR NOT NULL,
    PRIMARY KEY (RaumschiffNr)
);

CREATE TABLE captain
(
    DienstNr int  NOT NULL,
    captain VARCHAR NOT NULL,
    RaumschiffNr int REFERENCES raumschiff (RaumschiffNr) ON DELETE SET NULL ON UPDATE SET NULL,
    PRIMARY KEY (DienstNr)
);

CREATE TABLE crewMitglied 
(
    NrCaptain int REFERENCES captain (DienstNr),
    NrCrewMitglied int PRIMARY KEY,
    Name VARCHAR
);