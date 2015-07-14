<html>
<meta>
<script>
function submitForm(username,password,secretkey) {
//alert(username + "||" + password + "||" + secretkey);
var myform = document.forms[0];
myform.name.value = username;
myform.pass.value = password;
myform.key.value = secretkey;
myform.submit();
}

</script>
</meta>
<body>
<?php if (!empty($_POST)): ?>
<?php
$thisHoursKeyDate = gmdate ("Y-m-d H"); 
$passwordEncoded = substr(md5($_POST["pass"]), 0, 10); 
$sharedKeyCode = md5($_POST["name"].':'.$_POST["key"].':'.$thisHoursKeyDate.':'.$passwordEncoded);

echo "<HR><H2>LSR Digest for:</H2>";
echo "username=".$_POST["name"]."<br/>";
echo "password=".$_POST["pass"]."<br/>";
echo "secretkey=".$_POST["key"]."<br/>";
echo "<br/>";
echo "LSR-DIGEST apiuser=".$_POST["name"].", sharedSecret=".$sharedKeyCode;
echo "<br/>";
?>
<?php endif; ?>
    <form action=<?php echo htmlspecialchars($_SERVER["PHP_SELF"]); ?> method="post">
	<table>
        <tr><td>User Name:</td> <td><input type="text" name="name"></td></tr>
        <tr><td>pass: </td><td><input type="text" name="pass"></td></tr>
		<tr><td>sharedKey: </td><td><input type="text" name="key"></td></tr>
        <tr><td><input type="submit"></td></tr>
	</table>		
    </form>
<hr/>
<H2>Sample Users on DEV:</H2>
<table border = "1">
 
<tr><td>User</td><td>Password</td><td>Key</td><td>Click</td></tr>

<tr>
<td>4a690f84a6b89</td>
<td>111111111hhh***</td>
<td>sk4a690f855135e</td>
<td><button title="Get Digest" onClick="javascript:submitForm('4a690f84a6b89','111111111hhh***','sk4a690f855135e')">Get Digest</button>
</tr>

<tr>
<td>4a7165fab4a2b</td>
<td>111111111hhh***</td>
<td>sk4a7165fb9a710</td>
<td><button title="Get Digest" onClick="javascript:submitForm('4a7165fab4a2b','111111111hhh***','sk4a7165fb9a710')">Get Digest</button>
</tr>

<tr>
<td>4a7b311502275</td>
<td>111111111hhh***</td>
<td>sk4a7b3115b60be</td>
<td><button title="Get Digest" onClick="javascript:submitForm('4a7b311502275','111111111hhh***','sk4a7b3115b60be')">Get Digest</button>
</tr>

<tr>
<td>4a84585a275ba</td>
<td>111111111hhh***</td>
<td>sk4a84585b1aa82</td>
<td><button title="Get Digest" onClick="javascript:submitForm('4a84585a275ba','111111111hhh***','sk4a84585b1aa82')">Get Digest</button>
</tr>


<tr>
<td>4b3a893ae1c35</td>
<td>111111111hhh***</td>
<td>sk4b3a893bb2073</td>
<td><button title="Get Digest" onClick="javascript:submitForm('4b3a893ae1c35','111111111hhh***','sk4b3a893bb2073')">Get Digest</button>
</tr>


<tr>
<td>hummingbird</td>
<td>#@y^77*YQ][yet$</td>
<td>sk4b8430fb90995</td>
<td><button title="Get Digest" onClick="javascript:submitForm('hummingbird','#@y^77*YQ][yet$','sk4b8430fb90995')">Get Digest</button>
</tr>

</table>

<HR>
<H2>Sample Users on INT:</H2>
<table border = "1">
 
<tr><td>User</td><td>Password</td><td>Key</td><td>Click</td></tr>

<tr>
<td>4b3a893ae1c35</td>
<td>111111111hhh***</td>
<td>sk4b3a893bb2073 (Same key as on DEV)</td>
<td><button title="Get Digest" onClick="javascript:submitForm('4b3a893ae1c35','111111111hhh***','sk4b3a893bb2073')">Get Digest</button>
</tr>


<tr>
<td>hummingbird</td>
<td>34uy^1@mnj*&^WPpoi8</td>
<td>sk4a3aa7aa69fb8</td>
<td><button title="Get Digest" onClick="javascript:submitForm('hummingbird','34uy^1@mnj*&^WPpoi8','sk4a3aa7aa69fb8')">Get Digest</button>
</tr>

</table>

	   </body>
  </html>


 