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
            1007 => "not login",
            1008 => "upload error",
            1009 => "user has no books",
            1010 => "cannot log out",
            1011 => "password comfirmation error"
        );
        $_output = & load_class("Output", "core");
        $_result_data = array(
            "status" => $status_code,
            "msg"    => $status_msg[$status_code]
        );
        if ($data != null) {
            $_result_data['data'] = is_array($data) ? var_urlencode($data) : urlencode($data);
        }
        $_output->set_content_type('application/json;charset=utf-8');
        $_output->set_output(urldecode(json_encode($_result_data)));
    }

    function var_urlencode($data)
    {
        $ret_data = array();
        foreach ($data as $item) {
            if (is_array($item)) {
                $ret_data[] = var_urlencode($item);
            } else {
                $ret_data[] = urlencode($item);
            }
        }
        return $ret_data;
    }
}