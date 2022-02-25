google.load('elements', '1', {
    packages: "transliteration",
    callback: function () {
        onLoad();
    }
});
function onLoad() {

    var options = {
        sourceLanguage: google.elements.transliteration.LanguageCode.ENGLISH,
        destinationLanguage: google.elements.transliteration.LanguageCode.HINDI, // available option English, Bengali, Marathi, Malayalam etc.
        shortcutKey: 'ctrl+g',
        transliterationEnabled: true
    };

    var control = new google.elements.transliteration.TransliterationControl(options);   
    if ($("#txtHindiContent").has().prevObject.length > 0) {
        control.makeTransliteratable(['txtHindiContent']);
    }
    if ($("#txtHindiContent1").has().prevObject.length > 0) {
        control.makeTransliteratable(['txtHindiContent1']);
    } 

    if ($("#txtHindiContent2").has().prevObject.length > 0) {
        control.makeTransliteratable(['txtHindiContent2']);
    }
     if ($("#txtHindiContent3").has().prevObject.length > 0) {
        control.makeTransliteratable(['txtHindiContent3']);
    } 

    if ($("#Nameh").has().prevObject.length > 0) {        
        control.makeTransliteratable(['Nameh']);
    } 
    if ($("#FatherNameh").has().prevObject.length > 0) {
        control.makeTransliteratable(['FatherNameh']);
    }    
    if ($("#SpouseNameh").has().prevObject.length > 0) {
        control.makeTransliteratable(['SpouseNameh']);
    } 
    //if ($(".txtHindiContentClass").has().prevObject.length > 0) {
    //    control.makeTransliteratable(['txtHindiContentClass']);
    //} 

    
    //var control = new google.elements.transliteration.TransliterationControl(options);
    //control.makeTransliteratable(['txtHindiContent1']);
}
google.setOnLoadCallback(onLoad);