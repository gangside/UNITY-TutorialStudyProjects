<?php
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "unitybackendtutorial";

//varicables submited by user
$loginUser = $_POST["loginUser"];
$loginPass = $_POST["loginPass"];
$confirmPass = $_POST["confirmPass"];
$str = strcmp($loginPass, $confirmPass);


// 데이터베이스를 연결
// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}


//데이터베이스에 질의
$sql = "SELECT username FROM users WHERE username = '" .$loginUser."'";
$result = $conn->query($sql);

if ($result->num_rows > 0) {
    //Tell usewr that that name is already taken
    echo "Username is already taken. ";

}
else if($loginPass !== $confirmPass || empty($loginPass) || empty($confirmPass)){
    echo "inputed passwords are not equal or has null";
} else {
    echo "Create user... ";
    //Insert the user and password into the database.
    $sql2 = "INSERT INTO users (username, password, level, coins)
             VALUES ('" .$loginUser."', '" .$loginPass."', 1, 0)";
    if ($conn->query($sql2) === TRUE) {
        echo "New record created successfully";
    } else {
        echo "Error: " . $sql2 . "<br>" . $conn->error;
    }
}
$conn->close();

?>