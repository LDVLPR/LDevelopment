$(document).ready(function () {
    tinymce.init({
        selector: 'textarea',
        height: 300,
        theme: 'modern',
        plugins: [
		  'advlist autolink lists link image charmap print preview hr anchor pagebreak',
		  'searchreplace wordcount visualblocks visualchars codesample fullscreen',
		  'insertdatetime media nonbreaking save table contextmenu directionality',
		  'emoticons template paste textcolor colorpicker textpattern imagetools'
        ],
        toolbar: 'insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | ' +
			'bullist numlist outdent indent | link image | print preview media | forecolor backcolor emoticons | codesample | readmore',
        image_advtab: true,
        menubar: false,
        statusbar: false,
        setup: function (editor) {
            editor.addButton('readmore', {
                text: 'Read more',
                icon: false,
                onclick: function () {
                    editor.insertContent('[more]');
                }
            });
        }
    });

    $('#TagsIds').multiselect();
});