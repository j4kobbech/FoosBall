﻿/*

jQuery(window).load(function () {
    // ******************************************************************
    // Match View
    // 
    
    var $r1 = $('#red-player-1'),
        $b1 = $('#blue-player-1'),
        $playerSelects = $('.select-player'),
        $openForm = $('#open-submit-match-form'),
        $submitMatchForm = $('#submit-match-form');

    $openForm.on('click', function () {
        $submitMatchForm.slideToggle('fast', function() {
            $submitMatchForm.css({'overflow':''});
        });
    });

    $('#create-match-button').on('click', function (e) {
        var errm = "";
        if (!!$r1.val() === false || !!$b1.val() === false) {
            errm = 'Choose at least a Player 1 on each team';
        }
        if (!!errm === true) {
            e.preventDefault();
            displayErrorMessage(errm);
        }
    });

    var valueBeforeChange;
    var $thisSelect;
    
    $playerSelects.on('focus', function (e) {
        $thisSelect = $(e.target);
        valueBeforeChange = $thisSelect.find(':selected').val();
    }).on('change', function () {
        var $thisOption = $thisSelect.find(':selected');

        // reset options 
        $.each($('option[value="' + valueBeforeChange + '"]').not($thisOption), function (idx, element) {
            $(element).removeAttr('disabled');
        });
        
        // if the chosen option is default (empty) 
        if (!$thisOption.val() === false) {
            $.each($('option[value="' + $thisOption.val() + '"]').not($thisOption), function (idx, element) {
                $(element).attr('disabled','disabled');
            });
        }
        
        valueBeforeChange = $thisSelect.find(':selected').val();
    });

    $('.delete').on('click', 'a', function (e) {
        if (confirm("Delete this match?") === false ) {
            e.preventDefault();
            return false;
        }
        return undefined;
    });
    
    // ******************************************************************
    // SaveMatchResult View
    // 

    // Validation
    $('#submit-score-button').on('click', function (e) {
        var errm = "",
            $teamRedScore = $('#team-red-score'),
            $teamBlueScore = $('#team-blue-score');
        
        clearErrorMessage();

        if (!!$r1.val() === false || !!$b1.val() === false) {
            errm = 'Choose at least a Player 1 on each team';
        }

        if (!!errm === true) {
            e.preventDefault();
            displayErrorMessage(errm);
        }
        
        if ($teamRedScore.val() === $teamBlueScore.val()) {
            displayErrorMessage("A FoosBall Fight must have a winner and a loser. Please resolve.");
        }

        if (errorState()) {
            e.preventDefault();
        }
    });
    
    $('#team-red-score-slider').on('change', function (e) {
        $('#team-red-score').val(e.target.value);
    });
    
    $('#team-blue-score-slider').on('change', function (e) {
        $('#team-blue-score').val(e.target.value);
    });
});

*/
