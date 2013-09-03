jQuery(window).load(function() {

    /* ******************************************************************
     * Admin View
     */
    var $selectPlayer = $('#select-player'),
        $copyProdToStaging = $('#copy-prod-to-staging'),
        $replayMatches = $('#replay-matches'),
        $listEmails = $('#list-player-emails'),
        $appNameTextBox = $('#Settings_Name');

    $selectPlayer.on('change', function () {
        $.ajax({
            type: 'get',
            url: '/Account/Edit/',
            data: { id: $(this).children(':selected').attr('id') },
            success: function (data) {
                $('#player-data').html(data);
            }
        });
    });

    $copyProdToStaging.on('click', function (e) {
        e.preventDefault();
        toggleOverlay();

        $.ajax({
            type: 'post',
            url: '/Admin/CopyProdData/',
            success: function () {
                toggleOverlay();
                alert('Data has been copied.');
            }
        });
    });

    $replayMatches.on('click', function (e) {
        e.preventDefault();
        toggleOverlay();

        $.ajax({
            type: 'post',
            url: '/Admin/ReplayMatches/',
            success: function () {
                toggleOverlay();
                alert('Matches has been replayed.');
            }
        });
    });

    $listEmails.on('click', function (e) {
        e.preventDefault();
        toggleOverlay();

        $.ajax({
            type: 'get',
            url: '/Admin/GetPlayerEmails/',
            success: function (data) {
                toggleOverlay();
                var $textareaWrapper = $('#list-of-emails').html('').append('<textarea></textarea>');
                for (var index in data) {
                    $textareaWrapper.find('textarea').append(data[index]+',').focus();
                }
            }
        });
    });

    $appNameTextBox.focus();
});


function toggleDomainField($checkbox) {
    var $textBox = $('#Settings_Domain');

    if (!$checkbox.attr('checked')) {
        $textBox.attr('disabled', 'disabled');
    } else {
        $textBox.removeAttr('disabled').focus();
    }
}