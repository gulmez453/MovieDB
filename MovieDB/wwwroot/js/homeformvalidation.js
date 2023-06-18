function validateInput(inputId) {
    const textInput = document.getElementById(inputId);
    const submitButton = document.getElementById('filter-submit-button');
    const inputValue = textInput.value.trim();

    // Remove any non-digit characters from the input
    const numericInput = inputValue.replace(/\D/g, '');

    // Update the input field with the cleaned numeric value
    textInput.value = numericInput;

    // Enable or disable the submit button based on the input validity
    if (numericInput.length > 0) {
        submitButton.disabled = false;
    } else {
        submitButton.disabled = true;
    }
}