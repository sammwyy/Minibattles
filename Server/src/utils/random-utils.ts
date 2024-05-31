const characters =
  'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';

export function randomString(length: number): string {
  const charactersArray = Array.from(characters);
  let result = '';

  for (let i = 0; i < length; i++) {
    result +=
      charactersArray[Math.floor(Math.random() * charactersArray.length)];
  }

  return result;
}

export function generateRandomNumber(min: number, max: number): number {
  min = Math.ceil(min);
  max = Math.floor(max);
  return Math.floor(Math.random() * (max - min + 1)) + min;
}
