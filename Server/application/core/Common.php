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
            1001 => "missing arguments",
            1002 => "database error",
            1003 => "invalid username",
            1004 => "same username",
            1005 => "passwords cannot confirm",
            1006 => "user not exist",
            1007 => "wrong password",
            1008 => "not login",
            1009 => "cannot loggout",
            1010 => "no book matches",
            1011 => "file is not selected",
            1012 => "file exceeds limit",
            1013 => "file exceeds form limit",
            1014 => "file is partial",
            1015 => "no temporary directory",
            1016 => "unable to write file",
            1017 => "upload stopped by extension",
            1018 => "invalid filetype",
            1019 => "invalid filesize",
            1020 => "destination error",
            1021 => "bad filename",
            1022 => "no filepath",
            1023 => "no file"
        );
        $_output = & load_class("Output", "core");
        $_result_data = array(
            "status" => $status_code,
            "msg"    => $status_msg[$status_code]
        );
        if ($data != null && !empty($data)) {
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