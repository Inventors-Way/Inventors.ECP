signtool sign /fd sha256 /tr http://timestamp.sectigo.com /td sha256 /d "ECP Tester" %1
signtool verify /v /pa %1