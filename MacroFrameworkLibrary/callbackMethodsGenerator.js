
const generateWithParamsAction = (values) => {
  const types = values.map(value => `T${value}`).join(", ")
  const params = values.map(value => `p${value}`).join(", ")
  const typeParams = values.map(value => `T${value} p${value}`).join(", ")

  const lines = []
  lines.push(`public static void ExecuteAction<${types}>(Action<${types}> action, ${typeParams}, string errorMessage = "", Action onFail = null) {`)
  lines.push(`    try {`)
  lines.push(`        action?.Invoke(${params});`)
  lines.push(`    } catch (Exception e) {`)
  lines.push(`        OnCallbackFail(e, errorMessage, onFail);`)
  lines.push(`    }`)
  lines.push(`}\n`)

  console.log(lines.join("\n"))
}

const generateWithParamsFunc = (values) => {
  const types = values.map(value => `T${value}`).join(", ")
  const params = values.map(value => `p${value}`).join(", ")
  const typeParams = values.map(value => `T${value} p${value}`).join(", ")

  const lines = []
  lines.push(`public static Result ExecuteFunc<${types}, Result>(Func<${types}, Result> action, ${typeParams}, Result resultDefault = default, string errorMessage = "", Action onFail = null) {`)
  lines.push(`    try {`)
  lines.push(`        return action == null ? resultDefault : action(${params});`)
  lines.push(`    } catch (Exception e) {`)
  lines.push(`        OnCallbackFail(e, errorMessage, onFail);`)
  lines.push(`        return resultDefault;`)
  lines.push(`    }`)
  lines.push(`}\n`)

  console.log(lines.join("\n"))
}

let values = []

const count = 16

for (let i = 1; i <= count; i++) {
  values.push(i)
  generateWithParamsAction(values)
}

console.log("-------------------------------------------------------------")

values = []

for (let i = 1; i <= count; i++) {
  values.push(i)
  generateWithParamsFunc(values)
}