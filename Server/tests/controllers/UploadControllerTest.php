<?php if ( ! defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-24
 * Time: 下午6:38
 */

class UploadControllerTest extends CIUnit_TestCase {

    public function setUp() {
        $this->CI = set_controller("upload_controller");
        $this->CI->add_session("test_user_1");
    }
    public function tearDown() {
        $this->CI->del_session();
    }

    public function testDoUpload() {
        $dir = dirname(__DIR__);
        $realFilePath = $dir . "/test_files/file1.txt";
        $post_data = array(
            "userfile" => '@' . $realFilePath
        );
        $curl = curl_init();
        curl_setopt($curl, CURLOPT_URL, "http://localhost:9999/upload" );
        curl_setopt($curl, CURLOPT_POST, 1);
        curl_setopt($curl, CURLOPT_POSTFIELDS, $post_data);
        curl_exec($curl);
        curl_close($curl);
        $ret = $this->CI->do_upload();
        $this->assertEquals($ret, true);
    }

}