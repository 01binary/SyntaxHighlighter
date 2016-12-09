/*--------------------------------------------------------*\
|   ______    __   |
|  |  __  |  |  |  |
|  | |  | |  |  |  |
|  | !__! |  |  |  |
|  !______!  !__!  |  binary : tech art
|
|  @file Displays tooltips for all token spans on page load.
|----------------------------------------------------------
|  @author Valeriy Novytskyy
\*---------------------------------------------------------*/

'use strict';

loadTransformDefinition();
initTooltips();

/*
 * Initialize tooltips for all transformed token spans.
 * This covers any span with class name that starts with 'pl-'.
 */
function initTooltips() {
    $('span[class^="pl-"]').each(function () {
        $(this).mouseenter(function () {
            showTooltip($(this));
        });

        $(this).mouseleave(function () {
            hideTooltip($(this));
        });
    });
}

/*
 * Get transform description from the loaded transform definition.
 * @param {string} transformName - Name of transform to get description for.
 * @returns {string} The transform description.
 */
function getTransformDescription(transformName) {
    if (getTransformDescription.definition) {
        var transforms = getTransformDescription.definition.transforms;

        for (var transformId in transforms) {
            if (transforms[transformId].name === transformName) {
                return transforms[transformId].description;
            }
        }
    }

    return 'loading extra info...';
}

/*
 * Load transform definition.
 * When loading completes, the next tooltip shown will have the correct description.
 */
function loadTransformDefinition() {
    var path = $('pre').attr('data-transformpath');

    $.ajax({ url: path }).done(function (result) {
        getTransformDescription.definition = result;
    });
}

/*
 * Create a tooltip.
 * @returns The new tooltip element jQuery.
 */
function createTooltip() {
    var div = '<div></div>';

    return $(div)
        .attr('id', 'tip')
        .addClass('token-tooltip')
        .hide()
        .appendTo($('body'));
}

/*
 * Get or create tooltip.
 * @returns The tooltip element jQuery.
 */
function getTooltip() {
    const $tooltip = $('#tip');

    if ($tooltip.length === 0) {
        return createTooltip();
    }

    return $tooltip;
}

/*
 * Show the tooltip for an element and highlight the element.
 * @param {jQuery} $element - Element for which to show the tooltip.
 */
function showTooltip($element) {
    var $tooltip = getTooltip();

    $tooltip.html(tooltipContent($element));
    $element.addClass('token-highlighted');

    window.setTimeout(function () {
        var y = Math.max(0, $element.offset().top - $tooltip.height() - 20);
        var x = Math.max(0, $element.offset().left + $element.width() / 2 - $tooltip.width() / 2 - 18);

        $tooltip.css({ left: x, top: y }).stop().fadeIn();
    }, 100);
}

/*
 * Hide the tooltip and un-highlight the element.
 * @param {jQuery} $element - Element which to un-highlight.
 */
function hideTooltip($element) {
    $element.removeClass('token-highlighted');
    getTooltip().stop().fadeOut();
}

/*
 * Build tooltip content.
 * @returns {string} The tooltip content.
 */
function tooltipContent($element) {
    var tokenClass = $element.attr('class').replace('token-highlighted', '');
    var tokenTransform = $element.attr('data-transform');
    var tokenDescription = getTransformDescription(tokenTransform);

    var display =
        '<span class="token-class">' + tokenClass + '</span>' +
        '<br><span class="token-transform">' + tokenTransform + '</span>' +
        '<br><span class="token-description">' + tokenDescription + '</span>';
            
    return display;
}
