<?php
    require_once "connectioninfo.php";

    header("Content-Type: text/plain");

    $xmlDoc = new DOMDocument();
    $root = $xmlDoc->appendChild($xmlDoc->createElement("ArrayOfCafe"));
    $xmlDoc->formatOutput = TRUE;

    $connectionString = "Database=SidewalkCafes;Data Source=eu-cdbr-azure-north-a.cloudapp.net;User Id=bc60b77133bf77;Password=95fe3ea7";
    $connectionValues = ConnectionStringToArray($connectionString);

    $databaseConnection = new mysqli($connectionValues[DB_HOST], 
                                     $connectionValues[DB_USER], 
                                     $connectionValues[DB_PASSWORD], 
                                     $connectionValues[DB_NAME]);

    if (!$databaseConnection->connect_error)
    {
        $query = "SELECT EntityName, CamisTradeName, StreetAddress, CamisPhoneNumber, Location FROM swc";
        $statement = $databaseConnection->prepare($query);
        if ($statement)
        {
            $statement->execute();
            $statement->store_result();
            $statement->bind_result($name, $tradeName, $streetAddress, $phoneNumber, $location);
            while ($statement->fetch())
            {
                $link = $root->appendChild($xmlDoc->createElement("Cafe"));
                $link->appendChild($xmlDoc->createElement("Name", $name));
                $link->appendChild($xmlDoc->createElement("TradeName", $tradeName));
                $link->appendChild($xmlDoc->createElement("StreetAddress", $streetAddress));
                $link->appendChild($xmlDoc->createElement("PhoneNumber", $phoneNumber));
                $link->appendChild($xmlDoc->createElement("Location", $location));
            }
            $statement->close();
        }
        $databaseConnection->close();
    }
    echo $xmlDoc->saveXML();
?>