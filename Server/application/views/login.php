<!doctype html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>Login</title>
</head>
<body>
<?php echo form_open_multipart('/login'); ?>
<input type="text" name="username" size="20"/>
<input type="text" name="password" size="20">
<br/><br/>

<input type="submit" value="login"/>
</form>
<a type="button" href="/logout">loggout</a>
</body>
</html>