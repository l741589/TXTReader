<?php
/**
 * Created by JetBrains PhpStorm.
 * User: van
 * Date: 13-10-18
 * Time: 7:55 P.M.
 * To change this template use File | Settings | File Templates.
 */

require_once "load.php";

/**
 * @param $username
 * @param $password
 * @return null|int
 */

function get_user_id($username, $password) {
    global $db;

    $sql = "select id from users where username = %s and password = %s";
    $id = $db->get_val($db->prepare($sql, $username, $password));
    return $id;
}

/**
 * @param $file
 * @param $user_id
 * @return bool
 */
function save_file($file, $user_id) {
    global $db;

    if (!is_array($file))
        return false;

    $name = $file['name'];
    $size = $file['size'];
    $type = $file['type'];
    $tmp_file = $file['tmp_name'];

    // using 'or' is for that for now I haven't tried send a post
    if ($tmp_file or is_uploaded_file($tmp_file)) {
        $file_in = fopen($tmp_file, "rb");
        $file_data = bin2hex(fread($file_in, $size));
        fclose($file_in);
        $file_data  = addslashes($file_data);

        $db->db_connect();
        if ($db->insert("books", array("user_id"=>$user_id, "name"=>$name, "filedata"=>$file_data),
            array("%d", "%s", "%b")))
            return true;
    }
    return false;
}

function upload() {
    extract($_POST);
    isset($username) and isset($password) or die("error1");
    $id = get_user_id($username, $password);
    if ($id)
        die("error2");
    save_file($_FILES["file"], $id) or die("error3");
}

