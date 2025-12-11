USE [ABAK_NUEVO];
GO

-- Evita duplicar datos si ya existen
IF NOT EXISTS (SELECT 1 FROM dbo.ArticulosAyuda)
BEGIN
    INSERT INTO dbo.ArticulosAyuda
        (Titulo, Categoria, Resumen, ContenidoHtml, FechaPublicacion, Activo)
    VALUES
    -- Artículo 1
    (
        N'Cómo iniciar sesión en ABAK',
        N'Primeros pasos',
        N'Guía rápida para que los nuevos usuarios puedan iniciar sesión en el portal ABAK.',
        N'<h2>Paso 1: Abrir el portal</h2>
          <p>Ingresa a la dirección del portal ABAK que te haya proporcionado el equipo.</p>
          <h2>Paso 2: Ir a Iniciar sesión</h2>
          <p>En la parte superior derecha da clic en <strong>Iniciar sesión</strong>.</p>
          <h2>Paso 3: Capturar datos</h2>
          <p>Escribe tu correo registrado y contraseña y pulsa <strong>Iniciar sesión</strong>.</p>',
        GETUTCDATE(),
        1
    ),
    -- Artículo 2
    (
        N'Recuperar contraseña olvidada',
        N'Primeros pasos',
        N'Pasos para recuperar el acceso cuando el usuario ha olvidado su contraseña.',
        N'<h2>Opción "¿Olvidaste tu contraseña?"</h2>
          <p>En la pantalla de inicio de sesión da clic en el enlace correspondiente.</p>
          <h2>Correo de recuperación</h2>
          <p>Escribe el correo con el que te registraste. El sistema enviará un mensaje con un enlace.</p>
          <h2>Restablecer contraseña</h2>
          <p>Abre el enlace del correo y captura tu nueva contraseña.</p>',
        GETUTCDATE(),
        1
    ),
    -- Artículo 3
    (
        N'Acceso a Manual de ayuda, Capacitación y Material libre',
        N'Navegación',
        N'Descripción general de las tres secciones disponibles en el portal ABAK.',
        N'<h2>Manual de ayuda</h2>
          <p>Contiene documentación detallada del sistema: ejemplos, pantallas y preguntas frecuentes.</p>
          <h2>Capacitación</h2>
          <p>Incluye cursos, evaluaciones y material para el aprendizaje de los usuarios.</p>
          <h2>Material libre</h2>
          <p>Ofrece recursos gratuitos como checklists, guías rápidas y documentos descargables.</p>',
        GETUTCDATE(),
        1
    );
END
GO

-- Verifica que efectivamente haya filas
SELECT COUNT(*) AS TotalArticulos FROM dbo.ArticulosAyuda;
SELECT TOP 5 * FROM dbo.ArticulosAyuda;
