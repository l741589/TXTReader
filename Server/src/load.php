<?php
/**
 * Created by PhpStorm.
 * User: van
 * Date: 13-10-26
 * Time: 下午10:30
 */

require_once "config.php";

global /** @var $db db */
$db;

require_db();

function require_db() {
    global $db;
    require_once "db.php";

    $db = new db(DB_HOST, DB_NAME, DB_USER, DB_PASSWORD);
}
?>