import React from 'react'
import { useState } from 'react';
import { useUser } from '../hooks/useUser';
import { useServer } from '../hooks/useServer';
import { TextField, Button, Box, Stack, Typography } from '@mui/material';
import type { UserSessionInfo } from '../types/UserSessionInfo';
import { useNavigate } from 'react-router'

const Login = () => {

const { login } = useUser();
const { baseUrl } = useServer();
const [info, setInfo] = useState<string>("");
const [email,setEmail] = useState<string>("");
const [password,setPassword] = useState<string>("");
const navigator = useNavigate();

async function handleLogin() {

  setInfo("Sending data to server...");

  const res = await fetch(baseUrl+"/api/users/login", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      email,
      password,
    }),
  });

  if (!res.ok) {
    setInfo("Could not find an account with that password/email.");
    return;
  }
  else{
const data : UserSessionInfo = await res.json();
  console.log(data.email);

  login(data);

  if(data.roles.toLowerCase().includes("admin"))
    navigator("/my-courses")
    else
  if(data.roles.toLowerCase().includes("teacher"))
    navigator("/my-courses")
    else
  navigator("/recent-submissions");
  }

  
}

  return (
    <Stack spacing={4} direction="column" sx={{ minWidth: 200 }}>
    <Box 
      sx={{ display: 'flex', justifyContent: 'center'}}>
      <img alt="Elab logo"
      src="../images/elab-logo.png"/></Box>
     
    <TextField label="Email"
      value={email}
      onChange={(e) => setEmail(e.target.value)}></TextField>
    <TextField label="Password"
      value={password}
      onChange={(e) => setPassword(e.target.value)}></TextField>
    <Typography>{info}</Typography>
    <Button onClick={handleLogin}>Login</Button>
    </Stack>
  )
}

export default Login