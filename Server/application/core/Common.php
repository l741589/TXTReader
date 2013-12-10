<?php  if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-12-8
 * Time: 下午3:34
 */

if (!function_exists("show_result")) {
    function show_result($status_code, $data = null)
    {
        $status_msg = array(
            1000 => "success",
            1001 => "missing args",
            1002 => "database error",
            1003 => "invalid username",
            1004 => "username exist",
            1005 => "user not exist",
            1006 => "wrong password",
            1007 => "no login",
            1008 => "upload error",
            1009 => "user has no books",
            1010 => "cannot log out",
        );
        $_output = & load_class("Output", "core");
        $_result_data = array(
            "status" => $status_code,
            "msg"    => $status_msg[$status_code]
        );
        if ($data != null) {
            $data = is_array($data) ? $data : $data;
            $_result_data['data'] = $data;
        }
        echo $_output
            ->set_content_type('application/json')
            ->set_output(json_encode($_result_data))
            ->get_output();
        exit;
    }
}