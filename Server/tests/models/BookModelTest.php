<?php
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-12-5
 * Time: 上午2:27
 */

class BookModelTest extends CIUnit_TestCase {

    protected $_file_info;
    private $_book_model;

    public function __construct() {
        parent::__construct();
    }

    public function setUp() {
        parent::setUp();
        $this->CI->load->model('Book_Model');
        $this->_book_model = $this->CI->Book_Model;
    }

    public function testSaveBook() {
        $dir = dirname(__DIR__);
        $realFileDir = $dir . "/test_files/file1.txt";
        $this->_file_info = array(
            "user_id" => 88,
            "book_name" => "file.txt",
            "file_md5" => md5_file($realFileDir),
            "file_data" => mysql_real_escape_string(file_get_contents($realFileDir))
        );
        $ret = $this->_book_model->save_book($this->_file_info);
        $this->assertEquals(true, $ret);
    }

    public function testFindBooksByName()
    {

    }

    public function testGetBookById()
    {

    }
}