import { test, expect } from './fixtures/solution-center';
test('exemplo: chama API autenticada com o JWT', async ({ request, solutionCenterJwt }) => {
  const response = await request.get(
    'https://spw.app.foursys.com/backoffice-rf-hom/api/<SUBSTITUIR_PELO_ENDPOINT>',
    { headers: { Authorization: `Bearer ${solutionCenterJwt}` } },
  );
  expect(response.status()).not.toBe(401);
});
