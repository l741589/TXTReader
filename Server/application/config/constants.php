<?php  if (!defined('BASEPATH')) exit('No direct script access allowed');

/*
|--------------------------------------------------------------------------
| File and Directory Modes
|--------------------------------------------------------------------------
|
| These prefs are used when checking and setting modes when working
| with the file system.  The defaults are fine on servers with proper
| security, but you may wish (or even need) to change the values in
| certain environments (Apache running a separate process for each
| user, PHP under CGI with Apache suEXEC, etc.).  Octal values should
| always be used to set the mode correctly.
|
*/
define('FILE_READ_MODE', 0644);
define('FILE_WRITE_MODE', 0666);
define('DIR_READ_MODE', 0755);
define('DIR_WRITE_MODE', 0777);

/*
|--------------------------------------------------------------------------
| File Stream Modes
|--------------------------------------------------------------------------
|
| These modes are used when working with fopen()/popen()
|
*/

define('FOPEN_READ', 'rb');
define('FOPEN_READ_WRITE', 'r+b');
define('FOPEN_WRITE_CREATE_DESTRUCTIVE', 'wb'); // truncates existing file data, use with care
define('FOPEN_READ_WRITE_CREATE_DESTRUCTIVE', 'w+b'); // truncates existing file data, use with care
define('FOPEN_WRITE_CREATE', 'ab');
define('FOPEN_READ_WRITE_CREATE', 'a+b');
define('FOPEN_WRITE_CREATE_STRICT', 'xb');
define('FOPEN_READ_WRITE_CREATE_STRICT', 'x+b');

/*
 * Result code for returned information
 * */
define('RESULT_SUCCESS', 1000);
define('RESULT_MISSING_ARGS', 1001);
define('RESULT_DB_ERROR', 1002);
define('RESULT_INVALID_USERNAME', 1003);
define('RESULT_SAME_USERNAME', 1004);
define('RESULT_PASSWORDS_DIFFERENT', 1005);
define('RESULT_USER_NOT_EXIST', 1006);
define('RESULT_PASSWD_ERROR', 1007);
define('RESULT_NOT_LOGIN', 1008);
define('RESULT_CANNOT_LOGOUT', 1009);
define('RESULT_NO_BOOK', 1010);
define('RESULT_UPLOAD_FILE_NOT_SELECT', 1011);
define('RESULT_UPLOAD_FILE_EXCEEDS_LIMIT', 1012);
define('RESULT_UPLOAD_FILE_EXCEEDS_FORM_LIMIT', 1013);
define('RESULT_UPLOAD_FILE_PARTIAL', 1014);
define('RESULT_UPLOAD_NO_TEMP_DIR', 1015);
define('RESULT_UPLOAD_UNABLE_TO_WRITE_FILE', 1016);
define('RESULT_UPLOAD_STOPPED_BY_EXTENSION', 1017);
define('RESULT_UPLOAD_INVALID_FILETYPE', 1018);
define('RESULT_UPLOAD_INVALID_FILESIZE', 1019);
define('RESULT_UPLOAD_DESTINATON_ERROR', 1020);
define('RESULT_UPLOAD_BAD_FILENAME', 1021);
define('RESULT_UPLOAD_NO_FILEPATH', 1022);
define('RESULT_NO_FILE', 1023);

/* End of file constants.php */
/* Location: ./application/config/constants.php */