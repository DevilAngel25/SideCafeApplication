<?php
    require_once "connectioninfo.php";

    $name = $_GET["Name"];
    $tradeName = $_GET["TradeName"];
    $streetAddress = $_GET["StreetAddress"];
    $phoneNumber = $_GET["PhoneNumber"];
    $location = $_GET["Location"];

    $connectionString = "Database=SidewalkCafes;Data Source=eu-cdbr-azure-north-a.cloudapp.net;User Id=bc60b77133bf77;Password=95fe3ea7";
    $connectionValues = ConnectionStringToArray($connectionString);

    $databaseConnection = new mysqli($connectionValues[DB_HOST], 
                                     $connectionValues[DB_USER], 
                                     $connectionValues[DB_PASSWORD], 
                                     $connectionValues[DB_NAME]);

    if (!$databaseConnection->connect_error)
    {
        $query = "INSERT INTO swc (EntityName, CamisTradeName, StreetAddress, CamisPhoneNumber, Location) VALUES (?,?,?,?,?)";
        $statement = $databaseConnection->prepare($query);
        if ($statement)
        {
            $statement->bind_param('sssss', $name, $tradeName, $streetAddress, $phoneNumber, $location);
            if (!$statement->execute())
            {
                echo "Database error".$databaseConnection->error;
            }
            $statement->store_result();
            $statement->close();
        }
        $databaseConnection->close();
    }
?>