const formatter = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD'
});

window.addEventListener('message', event => {
  switch (event.data.type) {
    case 'id':
      setPlayerId(event.data.id)
      break
    case 'balance':
      setPlayerBalance(event.data.balance)
      break
    case 'postal':
      setPostal(event.data.postal)
      break
    case 'aop':
      setAop(event.data.aop)
      break
    case 'speedlimit':
      setSpeedLimit(event.data.speedlimit)
      break
  }
})

function setPlayerId(i) {
  document.getElementById("playerId").innerHTML = i;
}

function setPlayerBalance(b) {
  document.getElementById("moneyCounter").innerHTML = formatter.format(b).replaceAll('$', '')
}

function setAop(a) {
  document.getElementById("aop").innerHTML = a;
}

function setPostal(p) {
  document.getElementById("postal").innerHTML = p;
}

function setSpeedLimit(s) {
  document.getElementById("speedLimit").innerHTML = s;
}