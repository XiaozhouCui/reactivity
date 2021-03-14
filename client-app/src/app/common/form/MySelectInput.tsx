import { useField } from 'formik'
import { Form, Label, Select } from 'semantic-ui-react'

interface Props {
  placeholder: string
  name: string
  options: any
  label?: string
}

// Re-usable select input
const MySelectInput = (props: Props) => {
  const [field, meta, helpers] = useField(props.name)
  return (
    <Form.Field error={meta.touched && !!meta.error}>
      <label>{props.label}</label>
      <Select
        clearable
        options={props.options}
        value={field.value || null}
        onChange={(e, d) => {
            // e: event, d: data
            return helpers.setValue(d.value)
        }}
        onBlur={() => helpers.setTouched(true)}
        placeholder={props.placeholder}
      />
      {meta.touched && meta.error ? (
        <Label basic color='red'>
          {meta.error}
        </Label>
      ) : null}
    </Form.Field>
  )
}

export default MySelectInput
