<?php
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "unitybackendtutorial";

//User submited variables
$userID =  $_POST["userID"];


// 데이터베이스를 연결
// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

//데이터베이스에 질의
$sql = "SELECT itemID FROM usersitems WHERE userID = '" .$userID."'";
$result = $conn->query($sql);

if ($result->num_rows > 0) {
    // output data of each row
    $rows = array();
    while($row = $result->fetch_assoc()) {
        $rows[] = $row;
    }
    //반복 이후
    echo json_encode($rows);
} else {
    echo "0";
}
$conn->close();

?>