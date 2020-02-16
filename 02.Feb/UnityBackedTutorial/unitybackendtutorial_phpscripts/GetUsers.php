<?php
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "unitybackendtutorial";

// 데이터베이스를 연결
// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}
echo "Connected successfully, now we will show the users";


//데이터베이스에 질의
$sql = "SELECT username, level FROM users";
$result = $conn->query($sql);

if ($result->num_rows > 0) {
    // output data of each row
    while($row = $result->fetch_assoc()) {
        echo"<br>". "username: " . $row["username"]. " - levlel: " . $row["level"];
    }
} else {
    echo "0 results";
}
$conn->close();

?>