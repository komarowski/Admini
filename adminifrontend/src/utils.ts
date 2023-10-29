/**
 * Parse the date string from the database to the Long Date format.
 * @param dateString Date string from the database.
 * @returns Date string in Long Date format.
 */
export const getFormatDateString = (dateString: string): string => {
  const timestamp = Date.parse(dateString);
  if (!isNaN(timestamp)) {
    const date = new Date(timestamp);
    const resultDate = date.toDateString(); 
    return resultDate.substring(resultDate.indexOf(' ') + 1);
  }
  return '';
}

/**
 * Converts a string to an integer.
 * @param text A string to convert into a number.
 * @param defaultValue Default value if the conversion failed.
 * @returns Integer.
 */
export const convertStringToInt = (text: string | null, defaultValue: number): number => {
  if (text) {
    const result = parseInt(text);
    if (!isNaN(result)) {
      return result;
    }    
  }
  return defaultValue;
}