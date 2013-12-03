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
        $realFileDir = $dir . "/test_files/file1.txt";
        $_FILES = array(
            'userfile' => array(
                'name' => 'file1.txt',
                'type' => 'text/plain',
                'size' => filesize($realFileDir),
                'tmp_name' =>$realFileDir,
                'error' => 0
            )
        );
//        var_dump($_FILES);
        $ret = $this->CI->do_upload();
        $this->assertEquals($ret, true);
    }

    function is_uploaded_file($filename) {
        //Check only if file exists
        return file_exists(getcwd() . './tmp/' . $filename);
    }

    function move_uploaded_file($filename, $destination) {
        //Copy file
        return copy($filename, $destination);
    }
}